using System.Linq;
using System.Text.Json.Serialization;
using AuthModule.DTOs;
using AuthModule.Entities;
using AuthModule.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Helpers;
using Shared.Result;
using Shared.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
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
    
    public async Task<ApiResult<List<UpdateFullNameResponse>>> UpdateFullNameAsync(string accountName, string fullName, bool isUsePhone)
    {
        var response = new ApiResult<List<UpdateFullNameResponse>>()
        {
            Data = new List<UpdateFullNameResponse>(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (string.IsNullOrEmpty(accountName))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = ResponseResultEnum.InvalidInput.Text();
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

        if (string.IsNullOrEmpty(fullName))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập tên.";                
            return response;
        }
        try
        {
            var updatedFullNames = await _authRepository.UpdateFullName(accountName, fullName, isUsePhone);
            if (updatedFullNames == null || updatedFullNames.Count() <= 0)
            {
                response.Code = ResponseResultEnum.NoData.Value();
                response.Message = $"Không tìm thấy tài khoản {accountName}.";
                return response;
            }
            response.Data = updatedFullNames.Select(x => new UpdateFullNameResponse()
            {
                AccountID = x.AccountID,
                EmployeeAccountMapID = x.EmployeeAccountMapID,
                CompanyId = x.CompanyId,
            }).ToList();
            response.Code = ResponseResultEnum.Success.Value();
            response.Message = "Cập nhật tên thành công.";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateFullNameAsync Exception accountName {accountName}, fullName {fullName}, isUsePhone {isUsePhone}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
        }
        
        return response;
    }

    public async Task<ApiResult<ValidateAccountRequest>> SignupAsync(SignupRequest request, bool isUsePhone)
    {
        var response = new ApiResult<ValidateAccountRequest>()
        {
            Data = new ValidateAccountRequest(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (request == null)
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
        else if ((string.IsNullOrEmpty(request.Phone)  == true || string.IsNullOrEmpty(request.PhoneCode)) && isUsePhone == true)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập số điện thoại.";
            return response;
        }
        else if (string.IsNullOrEmpty(request.Mail) == false && isUsePhone == false)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng chỉ nhập số điện thoại hoặc mail.";
            return response;
        }

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

            var authdata = await _authRepository.Login(accountName, isUsePhone);
            if (authdata == null || authdata.AccountId <= 0)
            {
                response.Code = ResponseResultEnum.NoData.Value();
                response.Message = $"Tài khoản {accountName} không tồn tại.";
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

            response.Data = new ValidateAccountRequest()
            {
                Phone = isUsePhone ? request.Phone : "",
                PhoneCode = isUsePhone ? request.PhoneCode : "+84",
                Email = null,
                Name = null,
                Fullname = null,
                Provider = isUsePhone ? "phone" : "mail",
                IsNoOtpFlow = 1
            };
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

    public async Task<ApiResult<ValidateAccountRequest>> ValidateAccountAsync(SignupRequest request,bool isUsePhone)
    {
        var response = new ApiResult<ValidateAccountRequest>()
        {
            Data = new ValidateAccountRequest(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (request == null)
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

            var authdata = await _authRepository.Login(accountName, isUsePhone);
            if (authdata == null || authdata.AccountId <= 0)
            {
                response.Code = ResponseResultEnum.NoData.Value();
                response.Message = $"Tài khoản {accountName} không tồn tại.";
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

            response.Data = new ValidateAccountRequest()
            {
                Phone = isUsePhone ? request.Phone : "",
                PhoneCode = isUsePhone ? request.PhoneCode : "+84",
                Email = null,
                Name = null,
                Fullname = null,
                Provider = isUsePhone ? "phone" : "mail",
                IsNoOtpFlow = 1
            };
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
    public async Task<ApiResult<AuthResponse>> SigninAsync(SignupRequest request, bool isUsePhone)
    {
        var response = new ApiResult<AuthResponse>()
        {
            Data = new AuthResponse(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (request == null)
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

        try
        {
            var accountName = "";
            CompanyAccountMapEntities? company = new CompanyAccountMapEntities()
            {
                CompanyId = 0,
                EmployeesInfoId = 0,
                Role = UserRole.SystemAdmin.Value(),
            };

            response.Data.SigninMethods = new List<string>();
            if (isUsePhone)
            {
                accountName = request.Phone;
                response.Data.SigninMethods.Add("phone");
            }
            else
            {
                accountName = request.Mail;
                response.Data.SigninMethods.Add("mail");
            }

            var authdata = await _authRepository.Login(accountName, isUsePhone);
            if (authdata == null || authdata.AccountId <= 0)
            {
                string fullname = Helper.GenerateUniqueString(15);
                string phone = "";
                string email = "";

                if (isUsePhone)
                {
                    phone = accountName;
                    email = $"{accountName}@mail.com";
                }
                else
                {
                    email = accountName;
                    phone = Helper.GenerateUniqueNumber(11);
                }
                authdata = new LoginResultEntities()
                {
                    AccountId = await _authRepository.RegisterAccount(request.PhoneCode, phone, email, fullname, request.DeviceId),
                    IsActive = true,
                };
            }

            if (authdata.IsActive == false)
            {
                LoggerHelper.Warning($"LoginAsync email {accountName} này đã bị khóa.");
                response.Code = ResponseResultEnum.AccountLocked.Value();
                response.Message = $"Tài khoản {accountName} đã bị khóa.";
                response.Data = null;
                return response;
            }

            var list_companys = await _authRepository.GetCompanyByAccountId(authdata.AccountId);

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
                        Id = authdata.AccountId,
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
                    #region data mặc định
                    response.Data.User = new AuthUserResponse()
                    {
                        Name = company.EmployeesFullName,
                        Id = authdata.AccountId,
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
                    #endregion
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
                    Id = authdata.AccountId
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
                    Id = authdata.AccountId,

                };
                return response;
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
            var accountId = await _authRepository.RegisterAccount("",phone, email, fullname,"");
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
}