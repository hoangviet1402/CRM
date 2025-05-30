using System.Data;
using AuthModule.DTOs;
using AuthModule.Entities;
using Azure;
using Infrastructure.DbContext;
using Infrastructure.StoredProcedureMapperModule;
using Microsoft.Data.SqlClient;
using Shared.Entities;
using Shared.Helpers;

namespace AuthModule.Repositories;

public class AuthRepository : StoredProcedureMapperModule, IAuthRepository
{
    public AuthRepository(DatabaseConnection dbConnection)
        : base(dbConnection, "TanTam")
    {
    }

    public async Task<int> RegisterAccount(string phoneCode, string phone, string email, string fullname, string deviceId)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Phone", phone },
                { "@PhoneCode", phoneCode },
                { "@Email", email },
                { "@FullName", fullname },
                { "@DeviceId", deviceId }
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_Account_Register", parameters, outputParameters);
            return outputParameters.GetSafeInt32("@OutResult");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RegisterAccount Exception.", ex);
            return 0;
        }
    }

    public async Task<LoginResultEntities?> Login(string accountName, bool isUsePhone)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountName", accountName },
                { "@IsUsePhone", isUsePhone }
            };

            return await ExecuteStoredProcedureAsync<LoginResultEntities>("Ins_Account_Login", parameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"Login Exception.", ex);
            return new LoginResultEntities();
        }
    }

    public async Task<int> UpdatePass(int accountId, int companyId, string newPass, string oldPass, int needSetPassword)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountId", accountId },
                { "@CompanyId", companyId },
                { "@Pass", AESHelper.HashPassword(newPass) },
                { "@OldPass", needSetPassword == 0 ? AESHelper.HashPassword(oldPass) : "" },
                { "@NeedSetPassword", needSetPassword }
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_Account_UpdatePass", parameters, outputParameters);
            return outputParameters.GetSafeInt32("@OutResult");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdatePass Exception.", ex);
            return 0;
        }
    }

    public async Task<List<CompanyAccountMapEntities>> GetCompanyByAccountId(int accountId)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountId", accountId }
            };

            return await ExecuteStoredProcedureListAsync<CompanyAccountMapEntities>("Ins_Account_GetAllCompany", parameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetCompanyByAccountId Exception.", ex);
            return new List<CompanyAccountMapEntities>();
        }
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

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_Account_InsertTokens", parameters, outputParameters);
            return outputParameters.GetSafeInt32("@OutResult");
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

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_Account_RevokeToken", parameters, outputParameters);
            return outputParameters.GetSafeInt32("@OutResult");
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

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_Account_UpdateToken_JwtID", parameters, outputParameters);
            return outputParameters.GetSafeInt32("@OutResult");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateEmployeeJwtID Exception.", ex);
            return 0;
        }
    }

    public async Task<AccountTokenInfoEntities> GetTokenInfo(int accountId, int companyId)
    {
        var dataTable = new AccountTokenInfoEntities();
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountId", accountId },
                { "@CompanyId", companyId }
            };

            dataTable = await ExecuteStoredProcedureAsync<AccountTokenInfoEntities>("Ins_Account_GetTokensByEmployeeID", parameters);
            if (dataTable == null)
            {
                dataTable = new AccountTokenInfoEntities();
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetTokenInfo Exception.", ex);

        }
        return dataTable ?? new AccountTokenInfoEntities();
    }

    public async Task<List<Ins_Account_UpdateFullName_Result>> UpdateFullName(string AccountName, string  FullName, bool IsUsePhone)
    {
        var dataTable = new List<Ins_Account_UpdateFullName_Result>();
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountName", AccountName },
                { "@FullName", FullName },
                { "@IsUsePhone",IsUsePhone }
            };
            dataTable = await ExecuteStoredProcedureListAsync<Ins_Account_UpdateFullName_Result>("Ins_Account_UpdateFullName", parameters);
            if (dataTable == null)
            {
                dataTable = new List<Ins_Account_UpdateFullName_Result>();
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateFullName Exception.", ex);
        }
        
        return dataTable;
    }
} 