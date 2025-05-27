using Company.Entities;

namespace Company.Repositories;

public interface ICompanyRepository
{
    Task<int> CreateCompanyAsync(string fullName, string address);

    Task<IEnumerable<EmployeeCompany>> GetEmployeeCompanyAsync(int employeeId);

    Task<int> CreateBranchAsync(string name, int companyId);

    Task<int> CreateDepartmentAsync(string name, int branchId, int companyId);

    Task<int> CreatePositionAsync(string name, int departmentId, int companyId);

    Task<List<DepartmentCreatedResult>> CreateDepartmentInAllBranchesAsync(string name, int companyId);

    Task<List<int>> CreateBranchesOptimizedAsync(int companyId, List<string> branchNames);

    Task<List<DepartmentCreatedResult>> CreateDepartmentsInAllBranchesOptimizedAsync(int companyId, List<string> departmentNames);
} 