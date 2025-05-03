using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthModule.DTOs;
using AuthModule.Entities;
using AuthModule.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Enums;
using Shared.Helpers;
using Shared.Result;
using Shared.Settings;
using Shared.Utils;

namespace AuthModule.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
        _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
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

            // Xử lý token int employeeId, string role, int companyId, JwtSettings settings
            var accessToken = JwtHelper.GenerateAccessToken(employees.EmployeeId, employees.Role.GetValueOrDefault(0), employees.CompanyId.GetValueOrDefault(0), _jwtSettings);
            var refreshToken = JwtHelper.GenerateRefreshToken();
            int lifeTime = 60;
            var configValue = _configuration["Token:RefreshTokenLifeTime"];
            if (int.TryParse(configValue, out int parsedLifeTime))
            {
                lifeTime = parsedLifeTime;
            }
            var isUpdateOrInsertAccountToken =  await _authRepository.UpdateOrInsertEmployeeToken(employees.EmployeeId, accessToken , refreshToken,  lifeTime, ip , imie);

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

    public async Task<ApiResult<RefeshTokenResponse>> RefreshTokenAsync(string refreshToken, int employeID, string ip, string imie)
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

        try
        {
            // Implement refresh token logic here
            var tokenInfo = await _authRepository.GetTokenInfo(employeID);
            if (tokenInfo != null)
            {
                if(tokenInfo.RefreshToken.Equals(refreshToken, StringComparison.OrdinalIgnoreCase) ==false)
                {
                    LoggerHelper.Debug($"Token {refreshToken} employeID {employeID} not equals");
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
                var accessToken = JwtHelper.GenerateAccessToken(tokenInfo.Id, tokenInfo.Role, tokenInfo.CompanyId, _jwtSettings);
                response.Data = new RefeshTokenResponse()
                {
                    AccessToken = accessToken,
                };
                response.Code = ResponseCodeEnum.Success.Value();
                response.Message = "Thành công";
                return response;
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
                var isUpdateOrInsertAccountToken = await _authRepository.UpdateOrInsertEmployeeToken(employees.Id, "", "", 0, ip, imie);
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