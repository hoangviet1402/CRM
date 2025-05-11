using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Services;

public interface IAuthService
{
    Task<ApiResult<AuthResponse>> LoginAsync(string accountName, bool isUsePhone, string password, string ip, string imie);
    Task<ApiResult<RefeshTokenResponse>> RefreshTokenAsync(string refreshToken, string accessToken, int accountId, int companyId, string ip, string imie);
    Task<ApiResult<bool>> LogoutAsync(string refreshToken, int accountId, int companyId, string ip, string imie);
    Task<ApiResult<bool>> CreatePassFornewEmployeeAsync(int employeeAccountMapId, string newPass, string comfirmPass);

    Task<ApiResult<bool>> ChangePass(int employeeAccountMapId, string newPass, string oldPass);
} 