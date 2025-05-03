using Company.DTOs;
using Shared.Result;

namespace Company.Services;

public interface ICompanyService
{
    Task<ApiResult<int>> CreateCompanyAsync(CreateCompanyRequest request);
} 