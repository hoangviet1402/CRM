using AuthModule.DTOs;
using AuthModule.Entities;
using AuthModule.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Helpers;
using Shared.Result;
using Shared.Utils;
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

    public async Task<ApiResult<UpdateFullNameSigupResponse>> UpdateFullNameSigupAsync(string phone, string mail, string fullName, bool isUsePhone, string ip, string imie)
    {
        var response = new ApiResult<UpdateFullNameSigupResponse>()
        {
            Data = new UpdateFullNameSigupResponse(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (string.IsNullOrEmpty(fullName))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập tên.";
            return response;
        }
        try
        {
            var updatedFullNames = await _authRepository.UpdateFullName(phone, mail, fullName, isUsePhone);
            if (updatedFullNames == null || updatedFullNames.Count() <= 0)
            {
                response.Code = ResponseResultEnum.NoData.Value();
                response.Message = $"Không tìm thấy tài khoản với thông tin đã cung cấp.";
                return response;
            }
            var company = updatedFullNames.FirstOrDefault();
            response.Data = await GenerateAndSaveTokensAsync(
                company.AccountID ?? 0,
                0,
                company.CompanyId ?? 0,
                company.AccountRole ?? 0,
                ip,
                imie,
                (accessToken, refreshToken) => new UpdateFullNameSigupResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            );
            response.Code = ResponseResultEnum.Success.Value();
            response.Message = "Cập nhật tên thành công.";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateFullNameAsync Exception phone {phone} |mail {mail}, fullName {fullName}, isUsePhone {isUsePhone}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
        }

        return response;
    }

    public async Task<ApiResult<ValidateAccountResponse>> ValidateAccountAsync(ValidateAccountRequest request, bool isUsePhone)
    {
        var response = new ApiResult<ValidateAccountResponse>()
        {
            Data = new ValidateAccountResponse(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        try
        {
            var accountName = "";
            if (isUsePhone)
            {
                accountName = $"{request.PhoneCode}{request.Phone}";
            }
            else
            {
                accountName = request.Mail;
            }

            var authdata = await _authRepository.Validate(accountName ?? "", isUsePhone);
            if (authdata == null || authdata.AccountId <= 0)
            {
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = $"Tài khoản {accountName} không tồn tại.";
                response.Data = new ValidateAccountResponse()
                {
                    Phone = isUsePhone ? request.Phone : "",
                    PhoneCode = isUsePhone ? request.PhoneCode : "+84",
                    Email = isUsePhone == false ? request.Mail : "",
                    Fullname = "",
                    Name = "",
                    Provider = isUsePhone ? "phone" : "mail",
                    IsNoOtpFlow = isUsePhone ? 1 : 0
                };
            }
            else
            {
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = ResponseResultEnum.Success.Text();
                response.Data = new ValidateAccountResponse()
                {
                    AccountId = authdata.AccountId
                };
            }

        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"LoginAsync Exception {JsonConvert.SerializeObject(request)}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            response.Data = null;
        }

        return response;
    }
    public async Task<ApiResult<AuthResponse>> SigninAsync(SigninRequest request, bool isUsePhone, string ip, string imie)
    {
        var response = new ApiResult<AuthResponse>()
        {
            Data = new AuthResponse(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        try
        {
            var accountName = "";
            CompanyAccountMapEntities? company = new CompanyAccountMapEntities()
            {
                CompanyId = 0,
                EmployeesInfoId = 0,
                Role = UserRole.SystemAdmin.Value(),
            };
            var signinMethods = new List<string>();
            if (isUsePhone)
            {
                accountName = $"{request.PhoneCode}{request.Phone}";
                signinMethods.Add("phone");
            }
            else
            {
                accountName = request.Mail;
                signinMethods.Add("mail");
            }
            if (string.IsNullOrEmpty(accountName))
            {
                response.Code = ResponseResultEnum.InvalidInput.Value();
                response.Message = isUsePhone ? "Vui lòng nhập số điện thoại." : "Vui lòng nhập mail.";
                response.Data = null;
                return response;
            }

            var authdata = await _authRepository.Login(request.UserId ?? 0, request.ShopId ?? 0, request.Password != null ? AESHelper.HashPassword(request.Password) : "");
            if (authdata == null || authdata.AccountId <= 0)
            {
                response.Code = ResponseResultEnum.NoData.Value();
                response.Message = "Mật khẩu không chính xác.";
                response.Data = null;
                return response;
            }

            if (authdata.IsActive == false)
            {
                LoggerHelper.Warning($"LoginAsync email {accountName} này đã bị khóa.");
                response.Code = ResponseResultEnum.AccountLocked.Value();
                response.Message = $"Tài khoản {accountName} đã bị khóa.";
                response.Data = null;
                return response;
            }

            response = await GetDataAlterAsync(authdata.AccountId, isUsePhone, accountName, signinMethods);
            response.Data = await GenerateAndSaveTokensAsync(
                authdata.AccountId,
                authdata.EmployeesInfoId ?? 0,
                authdata.CompanyId,
                company.Role,
                ip,
                imie,
                (accessToken, refreshToken) => new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            );
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"LoginAsync Exception {JsonConvert.SerializeObject(request)}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            response.Data = null;
        }

        return response;
    }
    public async Task<ApiResult<AuthResponse>> GetDataAlterAsync(int accountId, bool isUsePhone, string accountName, List<string> signinMethods)
    {
        var response = new ApiResult<AuthResponse>()
        {
            Data = new AuthResponse(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (accountId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Thông tin không được xác thực.";
            return response;
        }

        try
        {
            response.Data.SigninMethods = signinMethods;

            CompanyAccountMapEntities? company = new CompanyAccountMapEntities()
            {
                CompanyId = 0,
                EmployeesInfoId = 0,
                Role = UserRole.SystemAdmin.Value(),
            };

            var list_companys = await _authRepository.GetCompanyByAccountId(accountId);

            // tài khoản đã được add 1 công ty
            if (list_companys.Count() == 1)
            {
                company = list_companys.FirstOrDefault();
                if (company == null) //Chưa tạo doanh nghiệp
                {
                    response.Code = ResponseResultEnum.CompanyNoData.Value();
                    response.Message = $"Tài khoản chưa được tạo bất kỳ doanh nghiệp nào.";
                    response.Data.User = new AuthUserResponse()
                    {
                        Id = accountId,
                    };
                    company = new CompanyAccountMapEntities()
                    {
                        CompanyId = 0,
                        EmployeesInfoId = 0,
                        Role = UserRole.SystemAdmin.Value(),
                    };
                }
                else // doanh nghiệp ok
                {
                    response.Data.User = new AuthUserResponse()
                    {
                        Name = company.EmployeesFullName,
                        Id = accountId,
                        ClientRole = company.Role.ToString(),
                        Phone = isUsePhone ? accountName : null,
                        Email = isUsePhone ? null : accountName,
                    };
                    response.Data.Company = new AuthCompanyResponse()
                    {
                        Name = company.CompanyFullName,
                        ShopUsername = company.CompanyFullName,
                        Id = company.CompanyId,
                        IsNewUser = company.IsNewUser,
                        NeedSetPassword = company.NeedSetPassword,
                        UserId = company.EmployeeAccountMapId,
                    };
                    response.Code = ResponseResultEnum.Success.Value();
                    response.Message = ResponseResultEnum.Success.Text();
                }
            }
            else if (list_companys.Count() > 1)// có nhiều hơn  1 doanh nghiệp
            {

                response.Data.ListCompanies = new List<AuthCompaniesResponse>();
                response.Data.ListCompanies = list_companys.Select(company => new AuthCompaniesResponse()
                {
                    ClientRole = company.Role.ToString(),
                    EmployeeName = company.EmployeesFullName,
                    UserId = company.EmployeesInfoId,
                    NeedSetPassword = company.NeedSetPassword,
                    Id = company.CompanyId,
                    Name = company.CompanyFullName,
                    IsNewUser = company.IsNewUser
                }).ToList();

                response.Data.User = new AuthUserResponse()
                {
                    Id = accountId
                };
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = $"Vui lòng chọn doanh nghiệp.";
            }
            else
            {
                response.Code = ResponseResultEnum.CompanyNoData.Value();
                response.Message = $"Tài khoản chưa được tạo bất kỳ doanh nghiệp nào.";
                response.Data.User = new AuthUserResponse()
                {
                    Id = accountId,
                };
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"LoginAsync Exception", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            response.Data = null;
        }

        return response;
    }
    public async Task<ApiResult<RefeshTokenResponse>> RefreshTokenAsync(string refreshToken, string jwtID, int accountId, int companyId, string ip, string imie)
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

        if (accountId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Thông tin không được xác thực.";
            return response;
        }

        try
        {
            // Implement refresh token logic here
            var tokenInfo = await _authRepository.GetTokenInfo(accountId, companyId);
            if (tokenInfo != null)
            {
                if (tokenInfo.RefreshToken.Equals(AESHelper.HashPassword(refreshToken), StringComparison.OrdinalIgnoreCase) == false)
                {
                    response.Code = ResponseResultEnum.InvalidToken.Value();
                    response.Message = $"Phiên đăng nhập Không tồn tại.";
                    return response;
                }
                else if (tokenInfo.JwtID.Equals(AESHelper.HashPassword(jwtID), StringComparison.OrdinalIgnoreCase) == false)
                {
                    response.Code = ResponseResultEnum.InvalidToken.Value();
                    response.Message = $"Phiên đăng nhập Không tồn tại.";
                    return response;
                }
                else if (tokenInfo.Expires < DateTime.Now)
                {
                    response.Code = ResponseResultEnum.TokenExpired.Value();
                    response.Message = $"Phiên đăng nhập hết hạn vui lòng đăng nhập lại.";
                    return response;
                }
                else if (!tokenInfo.AccountIsActive)
                {
                    response.Code = ResponseResultEnum.AccountLocked.Value();
                    response.Message = $"Tài Khoản nhân viên này hiện bị khóa.";
                    return response;
                }
                else if (tokenInfo.CompanyIsActive)
                {
                    response.Code = ResponseResultEnum.AccountLocked.Value();
                    response.Message = $"Công ty nhân viên đang làm việc hiện bị khóa.";
                    return response;
                }

                else if (tokenInfo.IsActive)
                {
                    response.Code = ResponseResultEnum.AccountLocked.Value();
                    response.Message = $"Nhân viên đang làm việc hiện bị khóa.";
                    return response;
                }

                // Xử lý tạo accessToken mới
                var newJwtID = "";
                var newAccessToken = JwtHelper.GenerateAccessToken(tokenInfo.AccountId, tokenInfo.EmployeesInfoId, tokenInfo.CompanyId, tokenInfo.Role, _configuration, out newJwtID);

                var isUpdateAccessToken = await _authRepository.UpdateEmployeeJwtID(tokenInfo.Id, newJwtID, ip, imie);

                if (isUpdateAccessToken > 0)
                {
                    response.Data = new RefeshTokenResponse()
                    {
                        AccessToken = newAccessToken,
                    };
                    response.Code = ResponseResultEnum.Success.Value();

                    return response;
                }
                else
                {
                    response.Code = ResponseResultEnum.AccountLocked.Value();
                    response.Message = $"Không tạo được token.";
                    return response;
                }
            }
            else
            {
                response.Code = ResponseResultEnum.InvalidToken.Value();
                response.Message = $"Phiên đăng nhập Không tồn tại.";
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RefreshTokenAsync Exception refreshToken {refreshToken},jwtID {jwtID}, accountId {accountId}, companyId {companyId}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            return response;
        }
    }
    public async Task<ApiResult<bool>> LogoutAsync(string refreshToken, int accountId, int companyId, string ip, string imie)
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
            var tokenInfo = await _authRepository.GetTokenInfo(accountId, companyId);
            if (tokenInfo != null && tokenInfo.RefreshToken.Equals(refreshToken, StringComparison.OrdinalIgnoreCase))
            {
                var isUpdateOrInsertAccountToken = await _authRepository.RevokeEmployeeToken(tokenInfo.Id, ip, imie);
                if (isUpdateOrInsertAccountToken > 0)
                {
                    response.Data = true;
                    response.Code = ResponseResultEnum.Success.Value();
                    return response;
                }
                else
                {
                    response.Code = ResponseResultEnum.InvalidToken.Value();
                    response.Message = $"Sai thông tin.";
                    return response;
                }
            }
            else
            {
                response.Code = ResponseResultEnum.NoData.Value();
                response.Message = $"Không tồn tại.";
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RefreshTokenAsync Exception  {refreshToken} int accountId {accountId}, int companyId {companyId}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            return response;
        }
    }
    public async Task<ApiResult<bool>> CreatePassFornewEmployeeAsync(int accountId, int companyId, string newPass, string comfirmPass)
    {
        var response = new ApiResult<bool>()
        {
            Data = false,
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (accountId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Bạn chưa chọn nhân viên.";
            return response;
        }

        if (companyId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Bạn chưa chọn công ty.";
            return response;
        }

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrEmpty(newPass))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập mật khẩu mới.";
            return response;
        }

        if (string.IsNullOrEmpty(comfirmPass))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng xác nhận mật khẩu.";
            return response;
        }

        if (comfirmPass.Equals(newPass))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Xác nhận mật khẩu không khớp với mật khẩu được nhập.";
            return response;
        }

        try
        {
            // Implement refresh token logic here
            var updatePass = await _authRepository.UpdatePass(accountId, companyId, newPass, "", 1);
            if (updatePass > 0)
            {
                response.Data = true;
                response.Code = ResponseResultEnum.Success.Value();
                return response;
            }
            else
            {
                response.Code = ResponseResultEnum.InvalidToken.Value();
                response.Message = $"Sai thông tin.";
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdatePassAsync Exception  {accountId} int newPass {newPass}, int comfirmPass {comfirmPass}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            return response;
        }
    }
    public async Task<ApiResult<bool>> ChangePass(int accountId, int companyId, string newPass, string oldPass)
    {
        var response = new ApiResult<bool>()
        {
            Data = false,
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (accountId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Bạn chưa chọn nhân viên.";
            return response;
        }

        if (companyId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Bạn chưa chọn công ty.";
            return response;
        }

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrEmpty(newPass))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập mật khẩu mới.";
            return response;
        }

        if (string.IsNullOrEmpty(oldPass))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập mật khẩu cũ.";
            return response;
        }

        if (oldPass.Equals(newPass))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Mật khẩu mới không được trùng với mật khẩu cũ.";
            return response;
        }

        try
        {
            // Implement refresh token logic here
            var updatePass = await _authRepository.UpdatePass(accountId, companyId, newPass, oldPass, 0);
            if (updatePass > 0)
            {
                response.Data = true;
                response.Code = ResponseResultEnum.Success.Value();
                return response;
            }
            else
            {
                response.Code = ResponseResultEnum.InvalidToken.Value();
                response.Message = $"Sai thông tin.";
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdatePassAsync Exception  {accountId} int newPass {newPass}, int comfirmPass {oldPass}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            return response;
        }
    }
    public async Task<ApiResult<int>> RegisterAccount(string accountName, string fullname, bool isUsePhone)
    {
        var response = new ApiResult<int>()
        {
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrEmpty(accountName))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập thông tin đăng ký.";
            return response;
        }

        try
        {
            string phone = "";
            string email = "";
            if (string.IsNullOrEmpty(fullname))
            {
                fullname = Helper.GenerateUniqueString(15);
            }

            if (isUsePhone == true)
            {
                phone = accountName;
                email = $"{accountName}@mail.com";
            }
            else
            {
                email = accountName;
                phone = Helper.GenerateUniqueNumber(11);
            }

            // Implement refresh token logic here
            var accountId = await _authRepository.RegisterAccount("", phone, email, fullname, "");
            if (accountId > 0)
            {
                response.Data = accountId;
                response.Code = ResponseResultEnum.Success.Value();
            }
            else
            {
                response.Code = ResponseResultEnum.InvalidData.Value();
                response.Message = $"Sai thông tin.";
            }
            return response;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RegisterAccount Exception accountName {accountName} int fullname {fullname}, int isUsePhone {isUsePhone}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            return response;
        }
    }

    public async Task<TResponse> GenerateAndSaveTokensAsync<TResponse>(int accountId, int employeeId, int companyId, int role, string ip, string imie,Func<string, string, TResponse> responseFactory)
    {
        string jwtID;
        var accessToken = JwtHelper.GenerateAccessToken(accountId, employeeId, companyId, role, _configuration, out jwtID);
        var refreshToken = JwtHelper.GenerateRefreshToken();
        int lifeTime = 30;
        var configValue = _configuration["Token:RefreshTokenLifeTime"];
        if (int.TryParse(configValue, out int parsedLifeTime))
        {
            lifeTime = parsedLifeTime;
        }
        await _authRepository.InsertEmployeeToken(employeeId, jwtID, refreshToken, lifeTime, ip, imie);
        return responseFactory(accessToken, refreshToken);
    }
}