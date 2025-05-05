using AuthModule.Entities;
using AuthModule.DTOs;
using Shared.Result;

namespace AuthModule.Repositories;

public interface IAuthRepository
{
    Task<LoginEntities> Login(string email, string password);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accessToken"></param>
    /// <param name="refreshToken"></param>
    /// <param name="lifeTime">số ngày refreshToken tồn tại</param>
    /// <param name="ip"></param>
    /// <param name="imie"></param>
    /// <returns></returns>
    Task<int> InsertEmployeeToken(int userId, string accessToken, string refreshToken, int lifeTime, string ip, string imie);
    Task<int> RevokeEmployeeToken(int employeeId, string ip, string imie);
    Task<int> UpdateEmployeeJwtID(int employeeId, string accessToken, string ip, string imie);
    Task<GetTokenInfoEntities> GetTokenInfo(int userId);
} 