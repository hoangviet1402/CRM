using EmployeeModule.DTOs;
using EmployeeModule.Entities;
using Shared.Result;

namespace EmployeeModule.Services;

public interface IEmployeeService
{
    Task<ApiResult<CreateEmployeeResponse>> CreateEmployeeSimple(string fullName, string employeeCode, string email, string phone);
    Task<EmployeeEntity?> GetEmployeeByIdAsync(int id);
} 