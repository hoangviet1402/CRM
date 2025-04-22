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

namespace AuthModule.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    public AuthService(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<ApiResult<AuthResponse>> LoginAsync(string email, string password)
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
            }

            if (!user.IsActive.GetValueOrDefault(false))
            {
                LoggerHelper.Warning($"LoginAsync email {email} này đã bị khóa.");
                response.Code = ResponseCodeEnum.AccountLocked.Value();
                response.Message = $"email {email} này đã bị khóa.";
            }

            // Xử lý token

            var token = GenerateJwtToken(user);
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            LoggerHelper.Information($"User {request.Username} logged in successfully");
            return new AuthResponse
            {
                Succeeded = true,
                AccessToken = token,
                ExpiresIn = DateTime.UtcNow.AddHours(1),
                Message = "Đăng nhập thành công"
            };
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"Login error for user {request.Username}", ex);
            return new AuthResponse 
            { 
                Succeeded = false,
                Message = "Đã có lỗi xảy ra khi đăng nhập" 
            };
        }
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
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

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("fullName", user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-256-bit-secret"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 