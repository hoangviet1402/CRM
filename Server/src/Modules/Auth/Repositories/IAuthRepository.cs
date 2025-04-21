using AuthModule.Entities;
using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Repositories;

public interface IAuthRepository
{
    Task<ApplicationUser> Login(string email, string password);
    Task<ApiResult<bool>> CreateUser(string username, string password, string email);
    Task<ApiResult<bool>> ChangePassword(int userId, string oldPassword, string newPassword);
    Task<int> CreateAccountForEmployee(string email, string password, int companyId, int role);
} 