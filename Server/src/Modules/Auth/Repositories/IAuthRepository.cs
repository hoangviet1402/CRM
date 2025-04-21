using AuthModule.Entities;
using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Repositories;

public interface IAuthRepository
{
    Task<ApiResult<bool>> Login(string username, string password);
    Task<ApiResult<bool>> CreateUser(string username, string password, string email);
    Task<ApiResult<bool>> ChangePassword(int userId, string oldPassword, string newPassword);
} 