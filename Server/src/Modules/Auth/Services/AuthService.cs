using AuthModule.DTOs;
using AuthModule.Repositories;
using Microsoft.Extensions.Configuration;
using Shared.Enums;
using Shared.Helpers;
using Shared.Result;

namespace AuthModule.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
    }

    public async Task<ApiResult<AuthResponse>> LoginAsync(string email, string password, string ip, string imie)
    {
        var response = new ApiResult<AuthResponse>()
        {
            Data = new AuthResponse(),
            Code = ResponseCodeEnum.SystemMaintenance.Value(),
            Message = ResponseCodeEnum.SystemMaintenance.Text()
        };

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Vui lòng nhập email.", nameof(email));
        }

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Vui lòng nhập mật khẩu.", nameof(password));
        }

        try
        {
            var employees = await _authRepository.Login(email,password);
            if(employees == null)
            {
                LoggerHelper.Warning($"LoginAsync email {email} không tồn tại");
                response.Code = ResponseCodeEnum.DataNotFound.Value();
                response.Message = $"Nhân viên {email} không tồn tại";
                return response;
            }
            else if(!employees.IsActive.GetValueOrDefault(false))
            {
                LoggerHelper.Warning($"LoginAsync email {email} này đã bị khóa.");
                response.Code = ResponseCodeEnum.AccountLocked.Value();
                response.Message = $"Nhân viên {email} đã bị khóa.";
                return response;
            }
            else if(employees.Role == null || employees.Role.GetValueOrDefault(0) <=0)
            {
                LoggerHelper.Warning($"LoginAsync email {email} này chưa được cấp quyền hệ thống.");
                response.Code = ResponseCodeEnum.InvalidRole.Value();
                response.Message = $"Nhân viên {email} chưa được cấp quyền hệ thống.";
                return response;
            }
            else if (!employees.CompanyIsActive.GetValueOrDefault(false))
            {
                LoggerHelper.Debug($"Company {employees.CompanyId} is not Active {email} employeID {employees.EmployeeId}");
                response.Code = ResponseCodeEnum.AccountLocked.Value();
                response.Message = $"Công ty nhân viên {email} đang làm việc hiện bị khóa.";
                return response;
            }

            // Xử lý token int employeeId, string role, int companyId, IConfiguration configuration
            var jwtID = JwtHelper.GenerateRefreshToken();
            var accessToken = JwtHelper.GenerateAccessToken(employees.EmployeeId, employees.Role.GetValueOrDefault(0), employees.CompanyId.GetValueOrDefault(0), jwtID, _configuration);
            var refreshToken = JwtHelper.GenerateRefreshToken();
            int lifeTime = 30;
            var configValue = _configuration["Token:RefreshTokenLifeTime"];
            if (int.TryParse(configValue, out int parsedLifeTime))
            {
                lifeTime = parsedLifeTime;
            }
            var isUpdateOrInsertAccountToken =  await _authRepository.InsertEmployeeToken(employees.EmployeeId, jwtID, refreshToken,  lifeTime, ip , imie);

            if(isUpdateOrInsertAccountToken > 0)
            {
                response.Data = new AuthResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
                response.Code = ResponseCodeEnum.Success.Value();
                response.Message = "Đăng nhập thành công";
                return response;
            }
            else
            {
                LoggerHelper.Debug($"Không tạo được token {email}");
                response.Code = ResponseCodeEnum.AccountLocked.Value();
                response.Message = $"Không tạo được token.";
                return response;
            }    
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"LoginAsync Exception {email}", ex);
            response.Code = ResponseCodeEnum.SystemError.Value();
            response.Message = ResponseCodeEnum.SystemError.Text(); 
            return response;
        }
    }

    public async Task<ApiResult<RefeshTokenResponse>> RefreshTokenAsync(string refreshToken,string accessToken, int employeID, string ip, string imie)
    {
        var response = new ApiResult<RefeshTokenResponse>()
        {
            Data = new RefeshTokenResponse(),
            Code = ResponseCodeEnum.SystemMaintenance.Value(),
            Message = ResponseCodeEnum.SystemMaintenance.Text()
        };

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Vui lòng nhập đủ thông tin.", nameof(refreshToken));
        }

        if (employeID <= 0)
        {
            throw new ArgumentException("Thông tin không được xác thực.", nameof(refreshToken));
        }

        try
        {
            // Implement refresh token logic here
            var tokenInfo = await _authRepository.GetTokenInfo(employeID);
            if (tokenInfo != null)
            {
                if(tokenInfo.RefreshToken.Equals(AESHelper.HashPassword(refreshToken), StringComparison.OrdinalIgnoreCase) ==false)
                {
                    LoggerHelper.Debug($"RefreshToken {refreshToken} employeID {employeID} not equals");
                    response.Code = ResponseCodeEnum.InvalidToken.Value();
                    response.Message = $"Phiên đăng nhập Không tồn tại.";
                    return response;
                }
                else if(tokenInfo.AccessToken.Equals(AESHelper.HashPassword(accessToken), StringComparison.OrdinalIgnoreCase) == false)
                {
                    LoggerHelper.Debug($"AccessToken {accessToken} employeID {employeID} not equals");
                    response.Code = ResponseCodeEnum.InvalidToken.Value();
                    response.Message = $"Phiên đăng nhập Không tồn tại.";
                    return response;
                }
                else if (tokenInfo.Expires < DateTime.Now)
                {
                    LoggerHelper.Debug($"Token {refreshToken} employeID {employeID} Expires");
                    response.Code = ResponseCodeEnum.ExpiredToken.Value();
                    response.Message = $"Phiên đăng nhập hết hạn vui lòng đăng nhập lại.";
                    return response;
                }
                else if (!tokenInfo.EmployeeIsActive)
                {
                    LoggerHelper.Debug($"employeID is not Active {refreshToken} employeID {employeID}");
                    response.Code = ResponseCodeEnum.AccountLocked.Value();
                    response.Message = $"Nhân viên này hiện bị khóa.";
                    return response;
                }
                else if (tokenInfo.CompanyIsActive)
                {
                    LoggerHelper.Debug($"Company {tokenInfo.CompanyId} is not Active {refreshToken} employeID {employeID}");
                    response.Code = ResponseCodeEnum.AccountLocked.Value();
                    response.Message = $"Công ty nhân viên đang làm việc hiện bị khóa.";
                    return response;
                }

                // Xử lý tạo accessToken mới
                var newJwtID = JwtHelper.GenerateRefreshToken();
                var newAccessToken = JwtHelper.GenerateAccessToken(tokenInfo.Id, tokenInfo.Role, tokenInfo.CompanyId , tokenInfo.AccessToken, _configuration);

                var isUpdateAccessToken = await _authRepository.UpdateEmployeeAccessToken(tokenInfo.Id, newJwtID, ip, imie);

                if (isUpdateAccessToken > 0)
                {
                    response.Data = new RefeshTokenResponse()
                    {
                        AccessToken = newAccessToken,
                    };
                    response.Code = ResponseCodeEnum.Success.Value();
                    response.Message = "Thành công";
                    return response;
                }
                else
                {
                    LoggerHelper.Debug($"RefreshTokenAsync fail tokenInfo.Id {tokenInfo.Id} refreshToken {refreshToken}");
                    response.Code = ResponseCodeEnum.AccountLocked.Value();
                    response.Message = $"Không tạo được token.";
                    return response;
                }  
            }
            else
            {
                LoggerHelper.Debug($"Không tồn tại {refreshToken} employeID {employeID}");
                response.Code = ResponseCodeEnum.InvalidToken.Value();
                response.Message = $"Phiên đăng nhập Không tồn tại.";
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RefreshTokenAsync Exception {refreshToken}", ex);
            response.Code = ResponseCodeEnum.SystemError.Value();
            response.Message = ResponseCodeEnum.SystemError.Text();
            return response;
        }
    }

    public async Task<ApiResult<bool>> LogoutAsync(string refreshToken, int employeID, string ip, string imie)
    {
        var response = new ApiResult<bool>()
        {
            Data = false,
            Code = ResponseCodeEnum.SystemMaintenance.Value(),
            Message = ResponseCodeEnum.SystemMaintenance.Text()
        };

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Vui lòng nhập đủ thông tin.", nameof(refreshToken));
        }

        try
        {
            // Implement refresh token logic here
            var employees = await _authRepository.GetTokenInfo(employeID);
            if (employees != null && employees.RefreshToken.Equals(refreshToken, StringComparison.OrdinalIgnoreCase))
            {
                var isUpdateOrInsertAccountToken = await _authRepository.RevokeEmployeeToken(employees.Id, ip, imie);
                if (isUpdateOrInsertAccountToken > 0)
                {
                    response.Data = true;
                    response.Code = ResponseCodeEnum.Success.Value();
                    response.Message = "Thành công";
                    return response;
                }
                else
                {
                    LoggerHelper.Debug($"Sai thông tin {refreshToken}");
                    response.Code = ResponseCodeEnum.InvalidToken.Value();
                    response.Message = $"Sai thông tin.";
                    return response;
                }
            }
            else
            {
                LoggerHelper.Debug($"Không tồn tại {refreshToken} employeID {employeID}");
                response.Code = ResponseCodeEnum.DataNotFound.Value();
                response.Message = $"Không tồn tại.";
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RefreshTokenAsync Exception  {refreshToken} employeID {employeID}", ex);
            response.Code = ResponseCodeEnum.SystemError.Value();
            response.Message = ResponseCodeEnum.SystemError.Text();
            return response;
        }
    }
} 