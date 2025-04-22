using System.Data;
using AuthModule.DTOs;
using AuthModule.Entities;
using Infrastructure.DbContext;
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

    public async Task<ApplicationUser> Login(string email, string password)
    {
        var connection = _context.Database.GetDbConnection();
        var response = new ApplicationUser();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_Login";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = AESHelper.HashPassword(password) });

            using var result = await command.ExecuteReaderAsync();
            if (await result.ReadAsync())
            {
                if (result.GetSafeInt32("Id") > 0)
                {
                    response = new ApplicationUser()
                    {
                        Id = result.GetSafeInt32("Id"),// result.GetInt32(result.GetOrdinal("Id")),
                        Email = result.GetSafeString("Email"),
                        CompanyId = result.GetSafeInt32("CompanyId"),
                        Role = result.GetSafeInt32("Role"),
                        CreatedAt = result.GetSafeDateTime("CreatedAt"),
                        IsActive = result.GetSafeBoolean("IsActive"),
                    };
                }
            }            
            return response;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }
    }

    public async Task<bool> CreateUser(string username, string password, string email)
    {
        var connection = _context.Database.GetDbConnection();
        var response = false;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "sp_CreateUser";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 100) { Value = username });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = AESHelper.HashPassword(password) });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email });

            await command.ExecuteNonQueryAsync();
            
            response = true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateUser Exception.", ex);           
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return response;
    }

    public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var connection = _context.Database.GetDbConnection();
        var response = false;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "sp_ChangePassword";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
            command.Parameters.Add(new SqlParameter("@OldPassword", SqlDbType.NVarChar, 256) { Value = AESHelper.HashPassword(oldPassword) });
            command.Parameters.Add(new SqlParameter("@NewPassword", SqlDbType.NVarChar, 256) { Value = AESHelper.HashPassword(newPassword) });

            var result = await command.ExecuteScalarAsync();
            
            response = Convert.ToBoolean(result);        
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"ChangePassword Exception.", ex);          
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return response;
    }

    public async Task<int> CreateAccountForEmployee(string email, string password, int companyId, int role)
    {
        var connection = _context.Database.GetDbConnection();
        var response = 0;       

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Account_CreateForEmployees";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = AESHelper.HashPassword(password) });
            command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int) { Value = companyId });
            command.Parameters.Add(new SqlParameter("@Role", SqlDbType.Int) { Value = role });
            command.Parameters.Add(new SqlParameter("@AccountId", SqlDbType.Int) { Direction = ParameterDirection.Output });

            await command.ExecuteNonQueryAsync();
            response = Convert.ToInt32(command.Parameters["@AccountId"].Value);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return response;
    }

    public async Task<int> UpdateOrInsertAccountToken(int userId, string accessToken, string refreshToken, int lifeTime, string ip, string imie)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_AccountTokens_UpdateOrInsert";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
            command.Parameters.Add(new SqlParameter("@AccessToken", SqlDbType.NVarChar, 258) { Value = accessToken });
            command.Parameters.Add(new SqlParameter("@RefreshToken", SqlDbType.NVarChar, 258) { Value = refreshToken });
            command.Parameters.Add(new SqlParameter("@LifeTime", SqlDbType.Int) { Value = lifeTime });
            command.Parameters.Add(new SqlParameter("@Ip", SqlDbType.VarChar, 100) { Value = ip });
            command.Parameters.Add(new SqlParameter("@Imie", SqlDbType.VarChar, 100) { Value = imie });
            command.Parameters.Add(new SqlParameter("@OutResult", SqlDbType.Int) { Direction = ParameterDirection.Output });

            await command.ExecuteNonQueryAsync();
            result = Convert.ToInt32(command.Parameters["@OutResult"].Value);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateOrInsertAccountToken Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return result;
    }

    public async Task<AccountTokenDto> GetAccountTokenByUserId(int userId)
    {
        var connection = _context.Database.GetDbConnection();
        AccountTokenDto result = new AccountTokenDto();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_AccountTokens_GetByUserID";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result = new AccountTokenDto
                {
                    Id = reader.GetSafeInt32("Id"),
                    UserId = reader.GetSafeInt32("UserId"),
                    AccessToken = reader.GetSafeString("AccessToken"),
                    RefreshToken = reader.GetSafeString("RefreshToken"),
                    Expires = reader.GetSafeDateTime("Expires"),
                    CreatedAt = reader.GetSafeDateTime("CreatedAt"),
                    CreatedByIp = reader.GetSafeString("CreatedByIp")
                };
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetAccountTokenByUserId Exception.", ex);
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