using AuthModule.Entities;
using AuthModule.DTOs;
using Shared.Result;
using Shared.Entities;

namespace AuthModule.Repositories;

public interface IAuthRepository
{
    Task<LoginResultEntities?> Login(int accountId, int companyId, string passwordHash);
    Task<LoginResultEntities?> Validate(string accountName, bool isUsePhone);
    Task<List<Ins_Account_UpdateFullName_Result>> UpdateFullName(string phone,string mail, string  FullName, bool IsUsePhone);
    Task<int> RegisterAccount(string phoneCode,string phone, string email, string fullname,string deviceId);
    Task<List<CompanyAccountMapEntities>> GetCompanyByAccountId(int accountId);
    Task<int> InsertEmployeeToken(int employeeId, string jwtID, string refreshToken, int lifeTime, string ip, string imie);
    Task<int> RevokeEmployeeToken(int employeeId, string ip, string imie);
    Task<int> UpdateEmployeeJwtID(int employeeId, string jwtID, string ip, string imie);
    Task<AccountTokenInfoEntities> GetTokenInfo(int accountId, int companyId);
    Task<int> UpdatePass(int accountId,int companyId, string newPass, string oldPass, int needSetPassword);
} 