using System.Linq;
using AuthModule.DTOs;
using AuthModule.Repositories;
using Microsoft.Extensions.Configuration;
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
    public async Task<ApiResult<AuthResponse>> LoginAsync(string accountName, bool isUsePhone, string password, string ip, string imie)
    {
        var response = new ApiResult<AuthResponse>()
        {
            Data = new AuthResponse(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (string.IsNullOrEmpty(accountName))
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
            response.Data.Model = isUsePhone ? "phone" : "mail";
            var authdata = await _authRepository.Login(accountName , isUsePhone, password);
            if(authdata == null || authdata.AccountId <= 0)
            {
                LoggerHelper.Warning($"LoginAsync {accountName} không tồn tại");
                response.Code = ResponseResultEnum.AccountNotExist.Value();
                response.Message = $"Tài khoản {accountName} không tồn tại";
                return response;
            }

            if (authdata.IsActive == false)
            {
                LoggerHelper.Warning($"LoginAsync email {accountName} này đã bị khóa.");
                response.Code = ResponseResultEnum.AccountLocked.Value();
                response.Message = $"Tài khoản {accountName} đã bị khóa.";
                return response;
            }

            var list_companys = await _authRepository.GetCompanyByAccountId(authdata.AccountId);

            // tài khoản đã được add 1 công ty
            if (list_companys.Count() == 1)
            {
                var company = list_companys.FirstOrDefault();

                // Doanh nghiệp được tạo bởi tài khoản này đã bị khóa
                if (company == null || (company.Role == 1 && company.IsActive == false))
                {
                    response.Code = ResponseResultEnum.CompanyLocked.Value();
                    response.Message = $"Doanh nghiệp của tài khoản {accountName} đã bị khóa.";
                    return response;
                }

                #region data mặc định
                response.Data.User = new AuthUserResponse()
                {
                    FullName = company.EmployeesFullName,
                    Id = company.AccountId,
                    EmployeesInfoId = company.EmployeesInfoId,
                    IsActive = company.IsActive
                };
                response.Data.Company = new AuthCompanyResponse()
                {
                    FullName = company.CompanyFullName,
                    IsActive = company.CompanyIsActive,
                    Id = company.CompanyId
                };
                #endregion

                // tài khoàn mới chưa tạo doanh nghiệp hoặc ko dc add vô doanh nghiệp nào
                if (company.IsNewUser)
                {
                    response.Code = ResponseResultEnum.Success.Value();
                    response.Data.User.IsNewUser = true;

                    response.Data.Company.NeedSetCompany = true;
                    response.Data.Company.NeedSetPassword = true;
                    response.Data.Company.CreateStep = company.CreateStep;
                    return response;
                }
                else // tài khoàn đã được đăng ký bởi ai đó hoặc đã tạo xong doanh nghiệp
                {
                    if (company.NeedSetPassword == true) // tài khoàn mới chưa tạo pass
                    {
                        response.Code = ResponseResultEnum.EmployeesNeedSetPass.Value();
                        response.Message = $"Vui lòng cập nhật mật khẩu.";
                        response.Data.Company = null;
                        response.Data.User = null;
                        return response;
                    }
                    else if (company.NeedSetPassword == false && string.IsNullOrEmpty(password)) // ko nhập pass
                    {
                        response.Code = ResponseResultEnum.InvalidInput.Value();
                        response.Message = $"Vui lòng nhập mật khẩu.";
                        response.Data.Company = null;
                        response.Data.User = null;
                        return response;
                    }
                    else if (company.NeedSetPassword == false && string.IsNullOrEmpty(password) == false) // có nhập pass
                    {
                        // nhập pass đúng
                        if (company.PasswordHash.Equals(AESHelper.HashPassword(password), StringComparison.CurrentCultureIgnoreCase))
                        {
                            // Xử lý token int employeeId, string role, int companyId, IConfiguration configuration
                            var jwtID = JwtHelper.GenerateRefreshToken();
                            var accessToken = JwtHelper.GenerateAccessToken(company.AccountId, company.EmployeesInfoId, company.Role, company.CompanyId, jwtID, _configuration);
                            var refreshToken = JwtHelper.GenerateRefreshToken();
                            int lifeTime = 30;
                            var configValue = _configuration["Token:RefreshTokenLifeTime"];
                            if (int.TryParse(configValue, out int parsedLifeTime))
                            {
                                lifeTime = parsedLifeTime;
                            }
                            var isUpdateOrInsertAccountToken = await _authRepository.InsertEmployeeToken(company.EmployeeAccountMapId, jwtID, refreshToken, lifeTime, ip, imie);

                            if (isUpdateOrInsertAccountToken > 0)
                            {
                                response.Data.AccessToken = accessToken;
                                response.Data.RefreshToken = refreshToken;
                                response.Code = ResponseResultEnum.Success.Value();
                                response.Message = "Đăng nhập thành công";
                                return response;
                            }
                            else
                            {
                                response.Code = ResponseResultEnum.AccountLocked.Value();
                                response.Message = $"Không tạo được token.";
                                response.Data.Company = null;
                                response.Data.User = null;
                                return response;
                            }
                        }
                        else // nhập pass sai
                        {
                            response.Code = ResponseResultEnum.InvalidPass.Value();
                            response.Message = $"Mật khẩu không đúng.";
                            response.Data.Company = null;
                            response.Data.User = null;
                            return response;
                        }
                    }
                }

                return response;
            }
            else // có nhiều hơn  1 doanh nghiệp
            {
                response.Data.Companys = new List<AuthCompanyResponse>();
                response.Data.Companys = list_companys.Select(company => new AuthCompanyResponse
                {
                    FullName = company.CompanyFullName,
                    IsActive = company.CompanyIsActive,
                    Id = company.CompanyId,
                    CreateStep = company.CreateStep
                }).ToList();
                response.Code = ResponseResultEnum.MoreThanOneCompany.Value();
                response.Message = $"Vui Lòng Chọn.";
                response.Data.Company = null;
                response.Data.User = null;
                return response;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"LoginAsync Exception {accountName}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text(); 
            return response;
        }
    }
    public async Task<ApiResult<RefeshTokenResponse>> RefreshTokenAsync(string refreshToken,string jwtID, int accountId, int companyId, string ip, string imie)
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
                if(tokenInfo.RefreshToken.Equals(AESHelper.HashPassword(refreshToken), StringComparison.OrdinalIgnoreCase) ==false)
                {
                    response.Code = ResponseResultEnum.InvalidToken.Value();
                    response.Message = $"Phiên đăng nhập Không tồn tại.";
                    return response;
                }
                else if(tokenInfo.JwtID.Equals(AESHelper.HashPassword(jwtID), StringComparison.OrdinalIgnoreCase) == false)
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
                var newJwtID = JwtHelper.GenerateRefreshToken();
                var newAccessToken = JwtHelper.GenerateAccessToken(tokenInfo.AccountId, tokenInfo.EmployeesInfoId, tokenInfo.CompanyId, tokenInfo.Role , newJwtID, _configuration);

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
    public async Task<ApiResult<bool>> CreatePassFornewEmployeeAsync(int employeeAccountMapId, string newPass , string comfirmPass)
    {
        var response = new ApiResult<bool>()
        {
            Data = false,
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

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
            var updatePass = await _authRepository.UpdatePass(employeeAccountMapId, newPass,"", 1);
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
            LoggerHelper.Error($"UpdatePassAsync Exception  {employeeAccountMapId} int newPass {newPass}, int comfirmPass {comfirmPass}", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
            return response;
        }
    }
    public async Task<ApiResult<bool>> ChangePass(int employeeAccountMapId, string newPass, string oldPass)
    {
        var response = new ApiResult<bool>()
        {
            Data = false,
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

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
            var updatePass = await _authRepository.UpdatePass(employeeAccountMapId, newPass, oldPass , 0);
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
            LoggerHelper.Error($"UpdatePassAsync Exception  {employeeAccountMapId} int newPass {newPass}, int comfirmPass {oldPass}", ex);
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

            if(isUsePhone == true)
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
            var accountId = await _authRepository.RegisterAccount(phone,email , fullname);
            if (accountId > 0)
            {
                response.Data = accountId;
                response.Code = ResponseResultEnum.Success.Value();
                return response;
            }
            else
            {
                response.Code = ResponseResultEnum.InvalidData.Value();
                response.Message = $"Sai thông tin.";
                return response;
            }
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