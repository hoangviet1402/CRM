using EmployeeModule.Entities;
using EmployeeModule.DTOs;
using Shared.Result;

namespace EmployeeModule.Repositories;

public interface IEmployeeRepository
{
    Task<EmployeeCreate_ResultEntities> EmployeeRegister(string fullname, string employeesCode, string phone, string email, string password, int companyId, int role);

    Task<EmployeeEntity?> GetEmployeeById(int id, int companyId);
} 