using EmployeeModule.DTOs;
using EmployeeModule.Entities;
using Shared.Result;

namespace EmployeeModule.Services;

public interface IEmployeeService
{
    Task<EmployeeEntity?> GetEmployeeByIdAsync(int id ,int companyId);
    Task<ApiResult<EmployeeCreate_ResultEntities>> CreateEmployeeAsync(string fullname, string phone, string email, string password, string employeeCode, int companyId, int role);
} 