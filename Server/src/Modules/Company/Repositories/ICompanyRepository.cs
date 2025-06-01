using Company.Entities;
using SqlMapper.Models;

namespace Company.Repositories;

public interface ICompanyRepository
{
    Task<int> CreateCompanyAsync(string fullName, string address);
    Task<IEnumerable<EmployeeCompany>> GetEmployeeCompanyAsync(int employeeId);
    Task<int> CreateBrancheAsync(CreateBranchesRequest request, int companyId);
    Task<int> CreateDepartmentAsync(string name, int branchId, int companyId);
    Task<int> CreatePositionAsync(string name, int departmentId, int companyId);
    Task<List<DepartmentCreatedResult>> CreateDepartmentInAllBranchesAsync(string name, int companyId);
    Task<List<DepartmentCreatedResult>> CreateDepartmentsInAllBranchesOptimizedAsync(int companyId, List<string> departmentNames);
    Task<int> UpdateCompanyStepAsync(int companyId, int code);
    Task<List<Ins_Business_GetList_Result>> BusinessGetListAsync();
    Task<int> UpdateInfoWhenSinup(UpdateInfoWhenSinupRequest request);
} 