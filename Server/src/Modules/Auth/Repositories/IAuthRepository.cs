using AuthModule.Entities;
using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Repositories;

public interface IAuthRepository
{
    Task<ApplicationUser> Login(string email, string password);
    Task<bool> CreateUser(string username, string password, string email);
    Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
    Task<int> CreateAccountForEmployee(string email, string password, int companyId, int role);
    
    // Add new methods
    Task<int> UpdateOrInsertAccountToken(int userId, string accessToken, string refreshToken, int lifeTime, string ip, string imie);
    Task<AccountTokenDto> GetAccountTokenByUserId(int userId);
} 