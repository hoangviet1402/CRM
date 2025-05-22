using System.Data;
using AuthModule.DTOs;
using AuthModule.Entities;
using Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Shared.Entities;
using Shared.Helpers;
using Infrastructure.StoredProcedureMapperModule;

namespace AuthModule.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly DatabaseConnection _dbConnection;
    private readonly StoredProcedureMapperModule _storedProcedureMapper;

    public AuthRepository(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
        _storedProcedureMapper = new StoredProcedureMapperModule();
    }

    public async Task<LoginResultEntities> Login(string accountName, bool isUsePhone, string password)
    {
        using var connection = _dbConnection.CreateConnection("Default");
        var response = new LoginResultEntities();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountName", accountName },
                { "@IsUsePhone", isUsePhone }
            };

            var result = await _storedProcedureMapper.ExecuteStoredProcedureWithResultAsync(connection, "Ins_Account_Login", parameters);

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
            throw;
        }

        return response;
    }

    public async Task<int> UpdatePass(int employeeAccountMapId, string newPass, string oldPass, int needSetPassword)
    {
        using var connection = _dbConnection.CreateConnection();
        var result = 0;

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@EmployeeAccountMapId", employeeAccountMapId },
                { "@Pass", AESHelper.HashPassword(newPass) },
                { "@OldPass", needSetPassword == 0 ? AESHelper.HashPassword(oldPass) : "" },
                { "@NeedSetPassword", needSetPassword }
            };

            var outputParameters = new Dictionary<string, object>();
            var success = await _storedProcedureMapper.ExecuteStoredProcedureAsync(connection, "Ins_Account_UpdatePass", parameters, outputParameters);

            if (success)
            {
                result = outputParameters.GetSafeInt32("@OutResult");
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdatePass Exception.", ex);
            throw;
        }

        return result;
    }

    public async Task<List<CompanyAccountMapEntities>> GetCompanyByAccountId(int accountId)
    {
        using var connection = _dbConnection.CreateConnection();
        var response = new List<CompanyAccountMapEntities>();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountId", accountId }
            };

            var result = await _storedProcedureMapper.ExecuteStoredProcedureWithResultAsync(connection, "Ins_Account_GetAllCompany", parameters);

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
            throw;
        }

        return response;
    }

    public async Task<int> InsertEmployeeToken(int employeeAccountMapId, string jwtID, string refreshToken, int lifeTime, string ip, string imie)
    {
        using var connection = _dbConnection.CreateConnection();
        var result = 0;

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

            var outputParameters = new Dictionary<string, object>();
            var success = await _storedProcedureMapper.ExecuteStoredProcedureAsync(connection, "Ins_Account_InsertTokens", parameters, outputParameters);

            if (success)
            {
                result = outputParameters.GetSafeInt32("@OutResult");
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"InsertEmployeeToken Exception.", ex);
            throw;
        }

        return result;
    }

    public async Task<int> RevokeEmployeeToken(int tokenId, string ip, string imie)
    {
        using var connection = _dbConnection.CreateConnection();
        var result = 0;

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Id", tokenId },
                { "@Ip", ip },
                { "@Imie", imie }
            };

            var outputParameters = new Dictionary<string, object>();
            var success = await _storedProcedureMapper.ExecuteStoredProcedureAsync(connection, "Ins_Account_RevokeToken", parameters, outputParameters);

            if (success)
            {
                result = outputParameters.GetSafeInt32("@OutResult");
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RevokeEmployeeToken Exception.", ex);
            throw;
        }

        return result;
    }

    public async Task<int> UpdateEmployeeJwtID(int tokenId, string jwtID, string ip, string imie)
    {
        using var connection = _dbConnection.CreateConnection();
        var result = 0;

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Id", tokenId },
                { "@JwtID", AESHelper.HashPassword(jwtID) },
                { "@Ip", ip },
                { "@Imie", imie }
            };

            var outputParameters = new Dictionary<string, object>();
            var success = await _storedProcedureMapper.ExecuteStoredProcedureAsync(connection, "Ins_Account_UpdateToken_JwtID", parameters, outputParameters);

            if (success)
            {
                result = outputParameters.GetSafeInt32("@OutResult");
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateEmployeeJwtID Exception.", ex);
            throw;
        }

        return result;
    }

    public async Task<AccountTokenInfoEntities> GetTokenInfo(int accountId, int companyId)
    {
        using var connection = _dbConnection.CreateConnection();
        var result = new AccountTokenInfoEntities();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@AccountId", accountId },
                { "@CompanyId", companyId }
            };

            var dataTable = await _storedProcedureMapper.ExecuteStoredProcedureWithResultAsync(connection, "Ins_Account_GetTokensByEmployeeID", parameters);

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
            throw;
        }

        return result;
    }
} 