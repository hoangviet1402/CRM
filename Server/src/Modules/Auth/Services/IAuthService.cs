using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Services;

public interface IAuthService
{
    Task<ApiResult<AuthResponse>> LoginAsync(string email, string password, string ip, string imie);
    Task<ApiResult<RefeshTokenResponse>> RefreshTokenAsync(string refreshToken, int employeID, string ip, string imie);
    Task<ApiResult<bool>> LogoutAsync(string refreshToken, int employeID, string ip, string imie);
} 