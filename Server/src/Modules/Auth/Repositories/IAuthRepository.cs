using AuthModule.Entities;
using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Repositories;

public interface IAuthRepository
{
    Task<ApplicationUser> Login(string email, string password);    
    Task<int> UpdateOrInsertEmployeeToken(int userId, string accessToken, string refreshToken, int lifeTime, string ip, string imie);
    Task<AccountTokenDto> GetAccountTokenByEmployeeId(int userId);
} 