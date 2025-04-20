using EmployeeModule.Entities;
using EmployeeModule.DTOs;
using Shared.Result;

namespace EmployeeModule.Repositories;

public interface IEmployeeRepository
{
    Task<ApiResult<CreateEmployeeResponse>> CreateEmployeeSimple(string fullName, string employeeCode, string email, string phone);
    Task<EmployeeEntity?> GetEmployeeById(int id);
} 