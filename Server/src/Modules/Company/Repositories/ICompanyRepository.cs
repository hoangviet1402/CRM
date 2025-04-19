namespace Company.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Entities.Company>> GetAllAsync();
    Task<Entities.Company?> GetByIdAsync(int id);
    Task<Entities.Company> CreateAsync(Entities.Company company);
    Task<Entities.Company> UpdateAsync(Entities.Company company);
    Task<bool> DeleteAsync(int id);
} 