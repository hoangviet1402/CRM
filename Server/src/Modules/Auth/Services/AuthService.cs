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
            var user = await _authRepository.Login(email,password);
            if (user == null)
            {
                LoggerHelper.Warning($"LoginAsync email {email} không tồn tại");
                response.Code = ResponseCodeEnum.DataNotFound.Value();
                response.Message = $"email {email} không tồn tại";
                return response;
            }

            if (!user.IsActive.GetValueOrDefault(false))
            {
                LoggerHelper.Warning($"LoginAsync email {email} này đã bị khóa.");
                response.Code = ResponseCodeEnum.AccountLocked.Value();
                response.Message = $"email {email} này đã bị khóa.";
                return response;
            }

            // Xử lý token
            var accessToken = JwtHelper.GenerateAccessToken(user.Id, user.Email ?? "", user.Role.GetValueOrDefault(0), user.CompanyId.GetValueOrDefault(0), _jwtSettings);
            var refreshToken = JwtHelper.GenerateRefreshToken();

            var isUpdateOrInsertAccountToken =  await _authRepository.UpdateOrInsertAccountToken(user.Id, accessToken , refreshToken, 30 , ip , imie);

            if(isUpdateOrInsertAccountToken > 0)
            {
                response.Data = new AuthResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccountID = user.Id
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

    public async Task<AuthResponse> RegisterAsync(string email,string phone, string password, int companyId, int role )
    {
        try
        {
            var existingUser = await _authRepository.CreateUser(username, password, email);
            if (existingUser == false)
            {
                LoggerHelper.Warning($"Registration failed: Username {request.Username} already exists");
                return new AuthResponse 
                { 
                    Succeeded = false,
                    Message = "Tên đăng nhập đã tồn tại" 
                };
            }

            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                Email = request.FullName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                LoggerHelper.Warning($"Registration failed for user {request.Username}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return new AuthResponse
                {
                    Succeeded = false,
                    Message = "Đăng ký không thành công",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            LoggerHelper.Information($"User {request.Username} registered successfully");
            return new AuthResponse
            {
                Succeeded = true,
                Message = "Đăng ký thành công"
            };
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"Registration error for user {request.Username}", ex);
            return new AuthResponse 
            { 
                Succeeded = false,
                Message = "Đã có lỗi xảy ra khi đăng ký" 
            };
        }
    }

    public Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        // Implement refresh token logic here
        throw new NotImplementedException();
    }

    public Task<bool> RevokeTokenAsync(string username)
    {
        // Implement revoke token logic here
        throw new NotImplementedException();
    }
} 