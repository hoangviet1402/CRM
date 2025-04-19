namespace Company.Repositories;

public class CompanyRepository : ICompanyRepository
{
    public async Task<IEnumerable<Entities.Company>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Entities.Company?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Entities.Company> CreateAsync(Entities.Company company)
    {
        throw new NotImplementedException();
    }

    public async Task<Entities.Company> UpdateAsync(Entities.Company company)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
} 