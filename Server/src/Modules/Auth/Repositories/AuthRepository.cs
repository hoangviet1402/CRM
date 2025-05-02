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
            command.CommandText = "Ins_Employee_Login";
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
  
    public async Task<int> UpdateOrInsertEmployeeToken(int employeeId, string accessToken, string refreshToken, int lifeTime, string ip, string imie)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_EmployeeTokens_UpdateOrInsert";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = employeeId });
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

    public async Task<AccountTokenDto> GetAccountTokenByEmployeeId(int employeeId)
    {
        var connection = _context.Database.GetDbConnection();
        AccountTokenDto result = new AccountTokenDto();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_EmployeeTokens_GetByEmployeeId";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = employeeId });

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result = new AccountTokenDto
                {
                    Id = reader.GetSafeInt32("Id"),
                    UserId = reader.GetSafeInt32("EmployeeId"),
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
            LoggerHelper.Error($"GetAccountTokenByEmployeeId Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return result;
    }

    public async Task<AccountTokenDto> GetRefreshToken(int userId)
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