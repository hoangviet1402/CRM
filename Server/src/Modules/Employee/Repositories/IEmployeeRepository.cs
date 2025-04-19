using EmployeeModule.Entities;
using EmployeeModule.DTOs;

namespace EmployeeModule.Repositories;

public interface IEmployeeRepository
{
    Task<int> CreateEmployeeSimple(string fullName, string employeeCode, string email, string phone);
    Task<EmployeeEntity?> GetEmployeeById(int id);
} 