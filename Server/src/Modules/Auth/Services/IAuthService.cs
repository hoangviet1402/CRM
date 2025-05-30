using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Services;

public interface IAuthService
{
    Task<ApiResult<List<UpdateFullNameResponse>>> UpdateFullNameAsync(string accountName, string fullName, bool isUsePhone);
    Task<ApiResult<ValidateAccountRequest>> ValidateAccountAsync(SignupRequest request,bool isUsePhone);
    Task<ApiResult<AuthResponse>> SigninAsync(SignupRequest request,bool isUsePhone);
    Task<ApiResult<ValidateAccountRequest>> SignupAsync(SignupRequest request, bool isUsePhone);
    Task<ApiResult<RefeshTokenResponse>> RefreshTokenAsync(string refreshToken, string accessToken, int accountId, int companyId, string ip, string imie);
    Task<ApiResult<bool>> LogoutAsync(string refreshToken, int accountId, int companyId, string ip, string imie);
    Task<ApiResult<bool>> CreatePassFornewEmployeeAsync(int accountId, int companyId, string newPass , string comfirmPass);
    Task<ApiResult<int>> RegisterAccount(string accountName, string fullname, bool isUsePhone);
    Task<ApiResult<bool>> ChangePass(int accountId, int companyId, string newPass, string oldPass);
} 