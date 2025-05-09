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
    public async Task<List<LoginResultEntities>> Login(string accountName, bool isUsePhone, string password)
    {
        var connection = _context.Database.GetDbConnection();
        var response = new List<LoginResultEntities>();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Employee_Login";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@AccountName", SqlDbType.NVarChar, 100) { Value = accountName });
            command.Parameters.Add(new SqlParameter("@IsUsePhone", SqlDbType.Bit) { Value = isUsePhone });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = AESHelper.HashPassword(password) });

            using var result = await command.ExecuteReaderAsync();
            if (result != null)
            {
                while (await result.ReadAsync())
                {
                    if (result != null && result.GetSafeInt32("Id") > 0)
                    {
                        response.Add(new LoginResultEntities()
                        {
                            EmployeeId = result.GetSafeInt32("EmployeeId"),
                            FullName = result.GetSafeString("FullName"),
                            Phone = result.GetSafeString("Phone"),
                            Email = result.GetSafeString("Email"),
                            PasswordHash = result.GetSafeString("PasswordHash"),
                            Role = result.GetSafeInt32("Role"),
                            CreatedAt = result.GetSafeDateTime("CreatedAt"),
                            IsActive = result.GetSafeBoolean("IsActive"),
                            IsNewUser = result.GetSafeInt32("IsNewUser"),
                            NeedSetPassword = result.GetSafeInt32("NeedSetPassword"),
                            NeedSetCompany = result.GetSafeInt32("NeedSetCompany"),
                            TotalCompany = result.GetSafeInt32("TotalCompany"),

                            CompanyName = result.GetSafeString("CompanyName"),
                            Alias = result.GetSafeString("Alias"),
                            Prefix = result.GetSafeString("Prefix"),
                            CreateDate = result.GetSafeDateTime("CreateDate"),
                            TotalEmployees = result.GetSafeInt32("TotalEmployees"),
                            CompanyIsActive = result.GetSafeBoolean("CompanyIsActive")
                        });
                    }
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
    public async Task<int> InsertEmployeeToken(int employeeId, string jwtID, string refreshToken, int lifeTime, string ip, string imie)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Employee_InsertTokens";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = employeeId });
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
    public async Task<int> RevokeEmployeeToken(int employeeId, string ip, string imie)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Employee_RevokeToken";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = employeeId });
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
    public async Task<int> UpdateEmployeeJwtID(int employeeId, string jwtID, string ip, string imie)
    {
        var connection = _context.Database.GetDbConnection();
        var result = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Employee_UpdateToken_JwtID";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = employeeId });
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
    public async Task<GetTokenInfoEntities> GetTokenInfo(int employeeID)
    {
        var connection = _context.Database.GetDbConnection();
        GetTokenInfoEntities result = new GetTokenInfoEntities();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Employee_GetTokensByEmployeeID";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = employeeID });

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result = new GetTokenInfoEntities
                {
                    Id = reader.GetSafeInt32("Id"),
                    EmployeeId = reader.GetSafeInt32("EmployeeId"),
                    EmployeeIsActive = reader.GetSafeBoolean("EmployeeIsActive"),
                    JwtID = reader.GetSafeString("JwtID"),
                    RefreshToken = reader.GetSafeString("RefreshToken"),
                    Expires = reader.GetSafeDateTime("Expires"),
                    CreatedAt = reader.GetSafeDateTime("CreatedAt"),
                    CreatedByIp = reader.GetSafeString("Ip"),
                    CompanyId = reader.GetSafeInt32("CompanyId"),
                    CompanyIsActive = reader.GetSafeBoolean("CompanyIsActive"),
                    
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