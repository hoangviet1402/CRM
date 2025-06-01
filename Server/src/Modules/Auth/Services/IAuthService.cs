using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Services;

public interface IAuthService
{
    Task<ApiResult<AuthResponse>> GetDataAlterAsync(int accountId, bool isUsePhone, string accountName, List<string> signinMethods);
    Task<ApiResult<UpdateFullNameSigupResponse>> UpdateFullNameSigupAsync(string phone, string mail, string fullName, bool isUsePhone, string ip, string imie);
    Task<ApiResult<ValidateAccountResponse>> ValidateAccountAsync(ValidateAccountRequest request,bool isUsePhone);
    Task<ApiResult<AuthResponse>> SigninAsync(SigninRequest request, bool isUsePhone, string ip, string imie);
    Task<ApiResult<RefeshTokenResponse>> RefreshTokenAsync(string refreshToken, string accessToken, int accountId, int companyId, string ip, string imie);
    Task<ApiResult<bool>> LogoutAsync(string refreshToken, int accountId, int companyId, string ip, string imie);
    Task<ApiResult<bool>> CreatePassFornewEmployeeAsync(int accountId, int companyId, string newPass , string comfirmPass);
    Task<ApiResult<int>> RegisterAccount(string accountName, string fullname, bool isUsePhone);
    Task<ApiResult<bool>> ChangePass(int accountId, int companyId, string newPass, string oldPass);
} 