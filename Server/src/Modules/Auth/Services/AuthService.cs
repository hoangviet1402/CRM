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

    public async Task<ApiResult<AuthResponse>> LoginAsync(string accountName, bool isUsePhone, string password, string ip, string imie)
    {
        var response = new ApiResult<AuthResponse>()
        {
            Data = new AuthResponse(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrEmpty(accountName))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            if (isUsePhone == true)
            {
                response.Message = $"Vui lòng nhập số điện thoại.";
            }
            else
            {
                response.Message = $"Vui lòng nhập mail.";
            }
            return response;
        }

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrEmpty(password))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = $"Vui lòng nhập mật khẩu.";
            return response;
        }

        try
        {
            var authdata = await _authRepository.Login(accountName , isUsePhone, password);
            if(authdata == null || authdata.Any() == false)
            {
                LoggerHelper.Warning($"LoginAsync {accountName} không tồn tại");
                response.Code = ResponseResultEnum.NoData.Value();
                response.Message = $"Nhân viên {accountName} không tồn tại";
                return response;
            }

            var employeeId = authdata.FirstOrDefault(){ 
                new  
            };
            else if(!authdata.IsActive)
            {
                LoggerHelper.Warning($"LoginAsync email {accountName} này đã bị khóa.");
                response.Code = ResponseResultEnum.AccountLocked.Value();
                response.Message = $"Nhân viên {accountName} đã bị khóa.";
                return response;
            }
            else if(authdata.Role <=0)
            {
                LoggerHelper.Warning($"LoginAsync {accountName} này chưa được cấp quyền hệ thống.");
                response.Code = ResponseResultEnum.NoData.Value();
                response.Message = $"Nhân viên {accountName} chưa được cấp quyền hệ thống.";
                return response;
            }

            var company = await _authRepository.Login(accountName, isUsePhone, password);

            // Xử lý token int employeeId, string role, int companyId, IConfiguration configuration
            var jwtID = JwtHelper.GenerateRefreshToken();
            var accessToken = JwtHelper.GenerateAccessToken(authdata.EmployeeId, authdata.Role, company.Role, jwtID, _configuration);
            var refreshToken = JwtHelper.GenerateRefreshToken();
            int lifeTime = 30;
            var configValue = _configuration["Token:RefreshTokenLifeTime"];
            if (int.TryParse(configValue, out int parsedLifeTime))
            {
                lifeTime = parsedLifeTime;
            }
            var isUpdateOrInsertAccountToken =  await _authRepository.InsertEmployeeToken(authdata.EmployeeId, jwtID, refreshToken,  lifeTime, ip , imie);

            if(isUpdateOrInsertAccountToken > 0)
            {


                response.Data = new AuthResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = new AuthUserResponse() {
                        id = authdata.EmployeeId,
                        fullname = authdata.FullName,
                        phone = authdata.Phone,
                        email = authdata.Email,
                        role = authdata.Role,
                        need_set_password = authdata.NeedSetPassword,
                        is_new_user = authdata.IsNewUser,
                        is_active = authdata.IsActive,
                    },
                    Company = new AuthCompanyResponse() { 
                    }
                };
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = "Đăng nhập thành công";
                return response;
            }
            else
            {
                LoggerHelper.Debug($"Không tạo được token {accountName}");
                response.Code = ResponseResultEnum.AccountLocked.Value();
                response.Message = $"Không tạo được token.";
                return response;
            }    
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"LoginAsync Exception {accountName}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text(); 
            return response;
        }
    }

    public async Task<ApiResult<RefeshTokenResponse>> RefreshTokenAsync(string refreshToken,string jwtID, int employeID, string ip, string imie)
    {
        var response = new ApiResult<RefeshTokenResponse>()
        {
            Data = new RefeshTokenResponse(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrEmpty(refreshToken))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập đủ thông tin.";
            return response;
        }

        if (employeID <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Thông tin không được xác thực.";
            return response;
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
                    response.Code = ResponseResultEnum.InvalidToken.Value();
                    response.Message = $"Phiên đăng nhập Không tồn tại.";
                    return response;
                }
                else if(tokenInfo.JwtID.Equals(AESHelper.HashPassword(jwtID), StringComparison.OrdinalIgnoreCase) == false)
                {
                    LoggerHelper.Debug($"AccessToken {jwtID} employeID {employeID} not equals");
                    response.Code = ResponseResultEnum.InvalidToken.Value();
                    response.Message = $"Phiên đăng nhập Không tồn tại.";
                    return response;
                }
                else if (tokenInfo.Expires < DateTime.Now)
                {
                    LoggerHelper.Debug($"Token {refreshToken} employeID {employeID} Expires");
                    response.Code = ResponseResultEnum.TokenExpired.Value();
                    response.Message = $"Phiên đăng nhập hết hạn vui lòng đăng nhập lại.";
                    return response;
                }
                else if (!tokenInfo.EmployeeIsActive)
                {
                    LoggerHelper.Debug($"employeID is not Active {refreshToken} employeID {employeID}");
                    response.Code = ResponseResultEnum.AccountLocked.Value();
                    response.Message = $"Nhân viên này hiện bị khóa.";
                    return response;
                }
                else if (tokenInfo.CompanyIsActive)
                {
                    LoggerHelper.Debug($"Company {tokenInfo.CompanyId} is not Active {refreshToken} employeID {employeID}");
                    response.Code = ResponseResultEnum.AccountLocked.Value();
                    response.Message = $"Công ty nhân viên đang làm việc hiện bị khóa.";
                    return response;
                }

                // Xử lý tạo accessToken mới
                var newJwtID = JwtHelper.GenerateRefreshToken();
                var newAccessToken = JwtHelper.GenerateAccessToken(tokenInfo.EmployeeId, tokenInfo.Role, tokenInfo.CompanyId , tokenInfo.JwtID, _configuration);

                var isUpdateAccessToken = await _authRepository.UpdateEmployeeJwtID(tokenInfo.EmployeeId, newJwtID, ip, imie);

                if (isUpdateAccessToken > 0)
                {
                    response.Data = new RefeshTokenResponse()
                    {
                        AccessToken = newAccessToken,
                    };
                    response.Code = ResponseResultEnum.Success.Value();
                    response.Message = "Thành công";
                    return response;
                }
                else
                {
                    LoggerHelper.Debug($"RefreshTokenAsync fail tokenInfo.Id {tokenInfo.EmployeeId} refreshToken {refreshToken}");
                    response.Code = ResponseResultEnum.AccountLocked.Value();
                    response.Message = $"Không tạo được token.";
                    return response;
                }  
            }
            else
            {
                LoggerHelper.Debug($"Không tồn tại {refreshToken} employeID {employeID}");
                response.Code = ResponseResultEnum.InvalidToken.Value();
                response.Message = $"Phiên đăng nhập Không tồn tại.";
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RefreshTokenAsync Exception {refreshToken}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            return response;
        }
    }

    public async Task<ApiResult<bool>> LogoutAsync(string refreshToken, int employeID, string ip, string imie)
    {
        var response = new ApiResult<bool>()
        {
            Data = false,
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrEmpty(refreshToken))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập đủ thông tin.";
            return response;
        }

        try
        {
            // Implement refresh token logic here
            var employees = await _authRepository.GetTokenInfo(employeID);
            if (employees != null && employees.RefreshToken.Equals(refreshToken, StringComparison.OrdinalIgnoreCase))
            {
                var isUpdateOrInsertAccountToken = await _authRepository.RevokeEmployeeToken(employees.EmployeeId, ip, imie);
                if (isUpdateOrInsertAccountToken > 0)
                {
                    response.Data = true;
                    response.Code = ResponseResultEnum.Success.Value();
                    response.Message = "Thành công";
                    return response;
                }
                else
                {
                    LoggerHelper.Debug($"Sai thông tin {refreshToken}");
                    response.Code = ResponseResultEnum.InvalidToken.Value();
                    response.Message = $"Sai thông tin.";
                    return response;
                }
            }
            else
            {
                LoggerHelper.Debug($"Không tồn tại {refreshToken} employeID {employeID}");
                response.Code = ResponseResultEnum.NoData.Value();
                response.Message = $"Không tồn tại.";
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RefreshTokenAsync Exception  {refreshToken} employeID {employeID}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            return response;
        }
    }
} 