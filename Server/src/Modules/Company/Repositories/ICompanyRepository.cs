namespace Company.Repositories;

public interface ICompanyRepository
{

    Task<int> CreateCompanyAsync(string fullName, string address);

    Task<IEnumerable<EmployeeCompany>> GetEmployeeCompanyAsync(int employeeId);
} 