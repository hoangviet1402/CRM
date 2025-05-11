using AuthModule.Entities;
using AuthModule.DTOs;
using Shared.Result;
using Shared.Entities;

namespace AuthModule.Repositories;

public interface IAuthRepository
{
    Task<LoginResultEntities> Login(string accountName, bool isUsePhone, string password);
    Task<List<CompanyAccountMapEntities>> GetCompanyByAccountId(int accountId);
    Task<int> InsertEmployeeToken(int employeeId, string jwtID, string refreshToken, int lifeTime, string ip, string imie);
    Task<int> RevokeEmployeeToken(int employeeId, string ip, string imie);
    Task<int> UpdateEmployeeJwtID(int employeeId, string jwtID, string ip, string imie);
    Task<AccountTokenInfoEntities> GetTokenInfo(int accountId, int companyId);
    Task<int> UpdatePass(int employeeAccountMapId, string newPass, string oldPass, int needSetPassword);
} 