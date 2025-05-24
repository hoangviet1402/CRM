using System.Data;
using AuthModule.DTOs;
using AuthModule.Entities;
using Infrastructure.DbContext;
using Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Shared.Entities;
using Shared.Helpers;
using Infrastructure.StoredProcedureMapperModule;

namespace AuthModule.Repositories;

public class AuthRepository : BaseRepository, IAuthRepository
{
    private readonly StoredProcedureMapperModule _storedProcedureMapper;

    public AuthRepository(DatabaseConnection dbConnection)
        : base(dbConnection, "TanCa")
    {
        _storedProcedureMapper = new StoredProcedureMapperModule();
    }

    public async Task<int> RegisterAccount(string phone, string email, string fullname)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Phone", phone },
                { "@Email", email },
                { "@FullName", fullname }
            };

            return await ExecuteStoredProcedureAsync("Ins_Account_Register", parameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RegisterAccount Exception.", ex);
            return 0;
        }
    }

    public async Task<LoginResultEntities> Login(string accountName, bool isUsePhone, string password)
    {
        var response = new LoginResultEntities();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountName", accountName },
                { "@IsUsePhone", isUsePhone }
            };

            var result = await ExecuteStoredProcedureWithResultAsync("Ins_Account_Login", parameters);

            if (result != null && result.Rows.Count > 0)
            {
                var row = result.Rows[0];
                response = new LoginResultEntities
                {
                    AccountId = row.GetSafeInt32("EmployeeId"),
                    Phone = row.GetSafeString("Phone"),
                    Email = row.GetSafeString("Email"),
                    CreatedAt = row.GetSafeDateTime("CreatedAt"),
                    IsActive = row.GetSafeBoolean("IsActive"),
                    TotalCompany = row.GetSafeInt32("IsNewUser")
                };
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"Login Exception.", ex);
        }

        return response;
    }

    public async Task<int> UpdatePass(int employeeAccountMapId, string newPass, string oldPass, int needSetPassword)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@EmployeeAccountMapId", employeeAccountMapId },
                { "@Pass", AESHelper.HashPassword(newPass) },
                { "@OldPass", needSetPassword == 0 ? AESHelper.HashPassword(oldPass) : "" },
                { "@NeedSetPassword", needSetPassword }
            };

            return await ExecuteStoredProcedureAsync("Ins_Account_UpdatePass", parameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdatePass Exception.", ex);
            return 0;
        }
    }

    public async Task<List<CompanyAccountMapEntities>> GetCompanyByAccountId(int accountId)
    {
        var response = new List<CompanyAccountMapEntities>();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountId", accountId }
            };

            var result = await ExecuteStoredProcedureWithResultAsync("Ins_Account_GetAllCompany", parameters);

            if (result != null)
            {
                foreach (DataRow row in result.Rows)
                {
                    if (row.GetSafeInt32("EmployeeAccountMapId") > 0)
                    {
                        response.Add(new CompanyAccountMapEntities
                        {
                            EmployeeAccountMapId = row.GetSafeInt32("EmployeeAccountMapId"),
                            AccountId = row.GetSafeInt32("AccountId"),
                            CompanyId = row.GetSafeInt32("CompanyId"),
                            EmployeesInfoId = row.GetSafeInt32("EmployeesInfoId"),
                            EmployeesFullName = row.GetSafeString("EmployeesFullName"),
                            Role = row.GetSafeInt32("Role"),
                            IsActive = row.GetSafeBoolean("IsActive"),
                            PasswordHash = row.GetSafeString("PasswordHash"),
                            IsNewUser = row.GetSafeBoolean("IsNewUser"),
                            NeedSetPassword = row.GetSafeBoolean("NeedSetPassword"),
                            CreatedAt = row.GetSafeDateTime("CreatedAt"),
                            CompanyFullName = row.GetSafeString("CompanyFullName"),
                            CompanyAlias = row.GetSafeString("Alias"),
                            CompanyPrefix = row.GetSafeString("Prefix"),
                            CompanyCreateDate = row.GetSafeDateTime("CompanyCreateDate"),
                            TotalEmployees = row.GetSafeInt32("TotalEmployees"),
                            CompanyIsActive = row.GetSafeBoolean("CompanyIsActive"),
                            CreateStep = row.GetSafeInt32("CreateStep")
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetCompanyByAccountId Exception.", ex);
        }

        return response;
    }

    public async Task<int> InsertEmployeeToken(int employeeAccountMapId, string jwtID, string refreshToken, int lifeTime, string ip, string imie)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@EmployeeAccountMapId", employeeAccountMapId },
                { "@JwtID", AESHelper.HashPassword(jwtID) },
                { "@RefreshToken", AESHelper.HashPassword(refreshToken) },
                { "@LifeTime", lifeTime },
                { "@Ip", ip },
                { "@Imie", imie }
            };

            return await ExecuteStoredProcedureAsync("Ins_Account_InsertTokens", parameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"InsertEmployeeToken Exception.", ex);
            return 0;
        }
    }

    public async Task<int> RevokeEmployeeToken(int tokenId, string ip, string imie)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Id", tokenId },
                { "@Ip", ip },
                { "@Imie", imie }
            };

            return await ExecuteStoredProcedureAsync("Ins_Account_RevokeToken", parameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RevokeEmployeeToken Exception.", ex);
            return 0;
        }
    }

    public async Task<int> UpdateEmployeeJwtID(int tokenId, string jwtID, string ip, string imie)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Id", tokenId },
                { "@JwtID", AESHelper.HashPassword(jwtID) },
                { "@Ip", ip },
                { "@Imie", imie }
            };

            return await ExecuteStoredProcedureAsync("Ins_Account_UpdateToken_JwtID", parameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateEmployeeJwtID Exception.", ex);
            return 0;
        }
    }

    public async Task<AccountTokenInfoEntities> GetTokenInfo(int accountId, int companyId)
    {
        var result = new AccountTokenInfoEntities();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountId", accountId },
                { "@CompanyId", companyId }
            };

            var dataTable = await ExecuteStoredProcedureWithResultAsync("Ins_Account_GetTokensByEmployeeID", parameters);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                result = new AccountTokenInfoEntities
                {
                    Id = row.GetSafeInt32("Id"),
                    JwtID = row.GetSafeString("JwtID"),
                    RefreshToken = row.GetSafeString("RefreshToken"),
                    Expires = row.GetSafeDateTime("Expires"),
                    Ip = row.GetSafeString("Ip"),
                    IsActive = row.GetSafeBoolean("IsActive"),
                    Role = row.GetSafeInt32("Role"),
                    AccountId = row.GetSafeInt32("AccountId"),
                    CompanyId = row.GetSafeInt32("CompanyId"),
                    EmployeesInfoId = row.GetSafeInt32("EmployeesInfoId"),
                    CompanyIsActive = row.GetSafeBoolean("CompanyIsActive"),
                    AccountIsActive = row.GetSafeBoolean("AccountIsActive")
                };
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetTokenInfo Exception.", ex);
        }

        return result;
    }
} 