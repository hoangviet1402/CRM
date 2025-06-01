using Company.DTOs;
using Company.Entities;
using Shared.Result;

namespace Company.Services;

public interface ICompanyService
{
    Task<ApiResult<int>> CreateCompanyAsync(CreateCompanyRequest request);
    Task<ApiResult<List<CreateBranchesResponse>>> CreateBranchesAsync(int companyId, List<CreateBranchesRequest> request);
    Task<ApiResult<int>> CreateDepartmentAsync(CreateDepartmentRequest request);
    Task<ApiResult<int>> CreatePositionAsync(CreatePositionRequest request);
    Task<ApiResult<List<DepartmentCreatedResult>>> CreateDepartmentInAllBranchesAsync(CreateDepartmentInAllBranchesRequest request);
    Task<ApiResult<List<ListBusinessResponse>>> ListBusinessResponseAsync();
    Task<ApiResult<int>> UpdateUserAndShopNameAsync(UpdateInfoWhenSinupRequest request);
}