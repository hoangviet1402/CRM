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

namespace AuthModule.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _context;

    public AuthRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResult<bool>> Login(string email, string password)
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
            command.CommandText = "sp_ValidateCredentials";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 100) { Value = email });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = HashPassword(password) });

            var result = await command.ExecuteScalarAsync();
            
            response.Data = Convert.ToBoolean(result);
            response.Code = ResponseCodeEnum.Success.Value();
            response.Message = response.Data ? "Xác thực thành công" : "Thông tin đăng nhập không chính xác";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"ValidateCredentials Exception.", ex);
            response.Code = ResponseCodeEnum.DatabaseError.Value();
            response.Message = "Lỗi xác thực thông tin đăng nhập";
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return response;
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
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = HashPassword(password) });
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
            command.Parameters.Add(new SqlParameter("@OldPassword", SqlDbType.NVarChar, 256) { Value = HashPassword(oldPassword) });
            command.Parameters.Add(new SqlParameter("@NewPassword", SqlDbType.NVarChar, 256) { Value = HashPassword(newPassword) });

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

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 