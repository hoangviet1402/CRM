namespace Company.Services;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDTO>> GetAllAsync();
    Task<CompanyDTO?> GetByIdAsync(int id);
    Task<CompanyDTO> CreateAsync(CompanyCreateDTO createDTO);
    Task<CompanyDTO> UpdateAsync(int id, CompanyDTO updateDTO);
    Task<bool> DeleteAsync(int id);
} 