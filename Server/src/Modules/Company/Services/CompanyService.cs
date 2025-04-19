namespace Company.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IEnumerable<CompanyDTO>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyDTO?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyDTO> CreateAsync(CompanyCreateDTO createDTO)
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyDTO> UpdateAsync(int id, CompanyDTO updateDTO)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
} 