using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Services;

public interface IAuthService
{
    Task<ApiResult<AuthResponse>> LoginAsync(string email, string password, string ip, string imie);
    Task<AuthResponse> RegisterAsync(string email, string phone, string password, int companyId, int role);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string username);
} 