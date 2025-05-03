using System.Data;
using System.Reflection.PortableExecutable;
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

    public async Task<LoginEntities> Login(string email, string password)
    {
        var connection = _context.Database.GetDbConnection();
        var response = new LoginEntities();

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
                if (result != null && result.GetSafeInt32("Id") > 0)
                {
                    response = new LoginEntities()
                    {
                        Id = result.GetSafeInt32("Id"),// result.GetInt32(result.GetOrdinal("Id")),
                        Email = result.GetSafeString("Email"),                        
                        Role = result.GetSafeInt32("Role"),
                        CreatedAt = result.GetSafeDateTime("CreatedAt"),
                        IsActive = result.GetSafeBoolean("IsActive"),
                        CompanyId = result.GetSafeInt32("CompanyId"),
                        CompanyIsActive = result.GetSafeBoolean("CompanyIsActive"),
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
            command.Parameters.Add(new SqlParameter("@AccessToken", SqlDbType.NVarChar, 258) { Value = AESHelper.HashPassword(accessToken) });
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
    public async Task<GetTokenInfoEntities> GetTokenInfo(int employeeID)
    {
        var connection = _context.Database.GetDbConnection();
        GetTokenInfoEntities result = new GetTokenInfoEntities();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_EmployeeTokens_GetByEmployeeID";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = employeeID });

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result = new GetTokenInfoEntities
                {
                    Id = reader.GetSafeInt32("Id"),
                    AccessToken = reader.GetSafeString("AccessToken"),
                    RefreshToken = reader.GetSafeString("RefreshToken"),
                    Expires = reader.GetSafeDateTime("Expires"),
                    CreatedAt = reader.GetSafeDateTime("CreatedAt"),
                    CreatedByIp = reader.GetSafeString("CreatedByIp"),
                    CompanyId = reader.GetSafeInt32("CompanyId"),
                    CompanyIsActive = reader.GetSafeBoolean("CompanyIsActive"),
                    EmployeeIsActive = reader.GetSafeBoolean("EmployeeIsActive"),
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