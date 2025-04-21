using AuthModule.DTOs;
using AuthModule.Entities;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Shared.Result;
using Shared.Helpers;
using Shared.Enums;
using System.Security.Cryptography;
using System.Text;
using System.Reflection.PortableExecutable;
using Shared.Entities;

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

    public async Task<ApiResult<bool>> CreateUser(string username, string password, string email)
    {
        var connection = _context.Database.GetDbConnection();
        var response = new ApiResult<bool>() 
        { 
            Code = ResponseCodeEnum.SystemMaintenance.Value(),
            Data = false
        };

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
            
            response.Data = true;
            response.Code = ResponseCodeEnum.Success.Value();
            response.Message = "Tạo tài khoản thành công";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateUser Exception.", ex);
            response.Code = ResponseCodeEnum.DatabaseError.Value();
            response.Message = "Tạo tài khoản thất bại";
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return response;
    }

    public async Task<ApiResult<bool>> ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var connection = _context.Database.GetDbConnection();
        var response = new ApiResult<bool>() 
        { 
            Code = ResponseCodeEnum.SystemMaintenance.Value(),
            Data = false
        };

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
            
            response.Data = Convert.ToBoolean(result);
            response.Code = ResponseCodeEnum.Success.Value();
            response.Message = response.Data ? "Đổi mật khẩu thành công" : "Mật khẩu cũ không chính xác";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"ChangePassword Exception.", ex);
            response.Code = ResponseCodeEnum.DatabaseError.Value();
            response.Message = "Đổi mật khẩu thất bại";
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

} 