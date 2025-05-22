using System.Data;
using AuthModule.DTOs;
using AuthModule.Entities;
using Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Entities;
using Shared.Helpers;

namespace AuthModule.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _context;

    public AuthRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<int> RegisterAccount(string phone,string email, string fullname)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_Register";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 100) { Value = phone });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 250) { Value = email });
            command.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 200) { Value = fullname });
            command.Parameters.Add(new SqlParameter("@OutResult", SqlDbType.Int) { Direction = ParameterDirection.Output });

            await command.ExecuteNonQueryAsync();
            result = Convert.ToInt32(command.Parameters["@OutResult"].Value);
            return result;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"RegisterAccount Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }
    }
    public async Task<LoginResultEntities> Login(string accountName, bool isUsePhone, string password)
    {
        var connection = _context.Database.GetDbConnection();
        var response = new LoginResultEntities();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_Login";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@AccountName", SqlDbType.NVarChar, 100) { Value = accountName });
            command.Parameters.Add(new SqlParameter("@IsUsePhone", SqlDbType.Bit) { Value = isUsePhone });
            //command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = AESHelper.HashPassword(password) });
            using var result = await command.ExecuteReaderAsync();
            if (await result.ReadAsync())
            {
                if (result != null && result.GetSafeInt32("Id") > 0)
                {
                    response= new LoginResultEntities()
                    {
                        AccountId = result.GetSafeInt32("AccountId"),

                        Phone = result.GetSafeString("Phone"),
                        Email = result.GetSafeString("Email"),

                        CreatedAt = result.GetSafeDateTime("CreatedAt"),
                        IsActive = result.GetSafeBoolean("IsActive"),
                        TotalCompany = result.GetSafeInt32("IsNewUser")
                    };
                }
            }
            return response;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"Login Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }
    }
    public async Task<int> UpdatePass(int employeeAccountMapId, string newPass,string oldPass, int needSetPassword)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_UpdatePass";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeAccountMapId", SqlDbType.Int) { Value = employeeAccountMapId });
            command.Parameters.Add(new SqlParameter("@Pass", SqlDbType.VarChar, 258) { Value = AESHelper.HashPassword(newPass) });
            command.Parameters.Add(new SqlParameter("@OldPass", SqlDbType.VarChar, 258) { Value = needSetPassword == 0 ? AESHelper.HashPassword(oldPass) : "" });
            command.Parameters.Add(new SqlParameter("@NeedSetPassword", SqlDbType.Int) { Value = needSetPassword });
            
            command.Parameters.Add(new SqlParameter("@OutResult", SqlDbType.Int) { Direction = ParameterDirection.Output });

            await command.ExecuteNonQueryAsync();
            result = Convert.ToInt32(command.Parameters["@OutResult"].Value);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateOrInsertEmployeesToken Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return result;
    }
    public async Task<List<CompanyAccountMapEntities>> GetCompanyByAccountId(int accountId)
    {
        var connection = _context.Database.GetDbConnection();
        var response = new List<CompanyAccountMapEntities>();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_GetAllCompany";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@AccountId", SqlDbType.Int) { Value = accountId });

            using var result = await command.ExecuteReaderAsync();
            if (result != null)
            {
                while (await result.ReadAsync())
                {
                    if (result != null && result.GetSafeInt32("EmployeeAccountMapId") > 0)
                    {
                        response.Add(new CompanyAccountMapEntities()
                        {
                            EmployeeAccountMapId = result.GetSafeInt32("EmployeeAccountMapId"),
                            AccountId = result.GetSafeInt32("AccountId"),
                            CompanyId = result.GetSafeInt32("CompanyId"),
                            EmployeesInfoId = result.GetSafeInt32("EmployeesInfoId"),
                            EmployeesFullName = result.GetSafeString("EmployeesFullName"),
                            Role = result.GetSafeInt32("Role"),
                            IsActive = result.GetSafeBoolean("IsActive"),
                            PasswordHash = result.GetSafeString("PasswordHash"),
                            IsNewUser = result.GetSafeBoolean("IsNewUser"),
                            NeedSetPassword = result.GetSafeBoolean("NeedSetPassword"),
                            CreatedAt = result.GetSafeDateTime("CreatedAt"),
                            CompanyFullName = result.GetSafeString("CompanyFullName"),
                            CompanyAlias = result.GetSafeString("Alias"),
                            CompanyPrefix = result.GetSafeString("Prefix"),
                            CompanyCreateDate = result.GetSafeDateTime("CompanyCreateDate"),
                            TotalEmployees = result.GetSafeInt32("TotalEmployees"),
                            CompanyIsActive = result.GetSafeBoolean("CompanyIsActive"),
                            CreateStep = result.GetSafeInt32("CreateStep")
                        });
                    }
                }
            }
            return response;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetCompanyByAccountId Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }
    }
    public async Task<int> InsertEmployeeToken(int employeeAccountMapId, string jwtID, string refreshToken, int lifeTime, string ip, string imie)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_InsertTokens";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeAccountMapId", SqlDbType.Int) { Value = employeeAccountMapId });
            command.Parameters.Add(new SqlParameter("@JwtID", SqlDbType.NVarChar, 258) { Value = AESHelper.HashPassword(jwtID) });
            command.Parameters.Add(new SqlParameter("@RefreshToken", SqlDbType.NVarChar, 258) { Value = AESHelper.HashPassword(refreshToken) });
            command.Parameters.Add(new SqlParameter("@LifeTime", SqlDbType.Int) { Value = lifeTime });
            command.Parameters.Add(new SqlParameter("@Ip", SqlDbType.VarChar, 100) { Value = ip });
            command.Parameters.Add(new SqlParameter("@Imie", SqlDbType.VarChar, 100) { Value = imie });
            command.Parameters.Add(new SqlParameter("@OutResult", SqlDbType.Int) { Direction = ParameterDirection.Output });

            await command.ExecuteNonQueryAsync();
            result = Convert.ToInt32(command.Parameters["@OutResult"].Value);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateOrInsertEmployeesToken Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return result;
    }
    public async Task<int> RevokeEmployeeToken(int tokenId, string ip, string imie)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_RevokeToken";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = tokenId });
            command.Parameters.Add(new SqlParameter("@Ip", SqlDbType.VarChar, 100) { Value = ip });
            command.Parameters.Add(new SqlParameter("@Imie", SqlDbType.VarChar, 100) { Value = imie });
            command.Parameters.Add(new SqlParameter("@OutResult", SqlDbType.Int) { Direction = ParameterDirection.Output });

            await command.ExecuteNonQueryAsync();
            result = Convert.ToInt32(command.Parameters["@OutResult"].Value);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateOrInsertEmployeesToken Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return result;
    }
    public async Task<int> UpdateEmployeeJwtID(int tokenId, string jwtID, string ip, string imie)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_UpdateToken_JwtID";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = tokenId });
            command.Parameters.Add(new SqlParameter("@JwtID", SqlDbType.NVarChar, 258) { Value = AESHelper.HashPassword(jwtID) });        
            command.Parameters.Add(new SqlParameter("@Ip", SqlDbType.VarChar, 100) { Value = ip });
            command.Parameters.Add(new SqlParameter("@Imie", SqlDbType.VarChar, 100) { Value = imie });
            command.Parameters.Add(new SqlParameter("@OutResult", SqlDbType.Int) { Direction = ParameterDirection.Output });

            await command.ExecuteNonQueryAsync();
            result = Convert.ToInt32(command.Parameters["@OutResult"].Value);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateOrInsertEmployeesToken Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return result;
    }
    public async Task<AccountTokenInfoEntities> GetTokenInfo(int accountId, int companyId)
    {
        var connection = _context.Database.GetDbConnection();
        AccountTokenInfoEntities result = new AccountTokenInfoEntities();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_GetTokensByEmployeeID";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@AccountId", SqlDbType.Int) { Value = accountId });
            command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int) { Value = companyId });

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result = new AccountTokenInfoEntities
                {
                    Id = reader.GetSafeInt32("Id"),
                    JwtID = reader.GetSafeString("JwtID"),
                    RefreshToken = reader.GetSafeString("RefreshToken"),
                    Expires = reader.GetSafeDateTime("Expires"),
                    Ip = reader.GetSafeString("Ip"),
                    IsActive = reader.GetSafeBoolean("IsActive"),
                    Role = reader.GetSafeInt32("Role"),
                    AccountId = reader.GetSafeInt32("AccountId"),
                    CompanyId = reader.GetSafeInt32("CompanyId"),
                    EmployeesInfoId = reader.GetSafeInt32("EmployeesInfoId"),
                    CompanyIsActive = reader.GetSafeBoolean("CompanyIsActive"),
                    AccountIsActive = reader.GetSafeBoolean("AccountIsActive")
                };
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetTokenInfo Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return result;
    }
} 