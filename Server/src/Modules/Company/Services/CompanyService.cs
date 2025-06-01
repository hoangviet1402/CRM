using Company.DTOs;
using Company.Repositories;
using Shared.Result;
using Shared.Enums;
using Shared.Helpers;
using System.Data;
using System.Numerics;

namespace Company.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ApiResult<int>> CreateCompanyAsync(CreateCompanyRequest request)
    {
        var response = new ApiResult<int>()
        {
            Data = 0,
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (string.IsNullOrEmpty(request.FullName))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập tên công ty.";
            return response;
        }

        if (string.IsNullOrEmpty(request.Address))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập địa chỉ công ty.";
            return response;
        }

        try
        {
            var companyId = await _companyRepository.CreateCompanyAsync(request.FullName, request.Address);

            if (companyId > 0)
            {
                response.Data = companyId;
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = "Tạo công ty thành công";
                return response;
            }

            response.Data = companyId;
            response.Code = ResponseResultEnum.Failed.Value();
            response.Message = "Tạo công ty thất bại";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateCompanyAsync Exception FullName {request.FullName}, Address {request.Address} ", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = "Tạo công ty thất bại";
        }

        return response;
    }

    public async Task<ApiResult<List<CreateBranchesResponse>>> CreateBranchesAsync(int companyId, List<CreateBranchesRequest> request)
    {
        var response = new ApiResult<List<CreateBranchesResponse>>()
        {
            Data = new List<CreateBranchesResponse>(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        try
        {
            var branchIds = 0;
            var branchId = 0;
            var now = DateTime.Now;
            foreach (var branch in request)
            {
                if (string.IsNullOrEmpty(branch.Name))
                {
                    branch.Name = "Chi nhánh " + (branchIds + 1);
                }

                if (string.IsNullOrEmpty(branch.Address))
                {
                    branch.Address = "Địa chỉ chi nhánh " + (branchIds + 1);
                }
                branchId = await _companyRepository.CreateBrancheAsync(branch, companyId);

                response.Data.Add(new CreateBranchesResponse
                {
                    Id = branchId,
                    Name = branch.Name,
                    AddressLat = branch.Latitude,
                    AddressLng = branch.Longitude,
                    Country = "",
                    Province = "",
                    District = "",
                    Address = branch.Address,
                    CreatedAt = now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Alias = TextHelper.NormalizeText(branch.Name, "-"),
                    Code = TextHelper.NormalizeText(branch.Name, "_").ToUpper(),
                    SortIndex = 0,
                    PhoneCode = 84,
                    Region = new RegionInfo
                    { 
                        Id = companyId
                    }
                });
                branchIds++;
            }           

            if (branchIds == request.Count())
            {
                var result = await _companyRepository.UpdateCompanyStepAsync(companyId, SetupStep.ONBOARDING_CREATE_BRANCH.Value());
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = "Tạo chi nhánh thành công";
                return response;
            }

            response.Code = ResponseResultEnum.Failed.Value();
            response.Message = "Tạo chi nhánh thất bại";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateBranchesAsync Exception CompanyId {companyId},", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = "Tạo chi nhánh thất bại";
        }

        return response;
    }    
    public async Task<ApiResult<int>> CreateDepartmentAsync(CreateDepartmentRequest request)
    {
        var response = new ApiResult<int>()
        {
            Data = 0,
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (string.IsNullOrEmpty(request.Name))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập tên phòng ban.";
            return response;
        }

        if (request.BranchId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "ID chi nhánh không hợp lệ.";
            return response;
        }

        if (request.CompanyId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "ID công ty không hợp lệ.";
            return response;
        }

        try
        {
            var departmentId = await _companyRepository.CreateDepartmentAsync(request.Name, request.BranchId, request.CompanyId);

            if (departmentId > 0)
            {
                var result = await _companyRepository.UpdateCompanyStepAsync(SetupStep.ONBOARDING_CREATE_DEPARTMENT.Value(), request.CompanyId);
                response.Data = departmentId;
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = "Tạo phòng ban thành công";
                return response;
            }

            response.Data = departmentId;
            response.Code = ResponseResultEnum.Failed.Value();
            response.Message = "Tạo phòng ban thất bại";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartmentAsync Exception Name {request.Name}, BranchId {request.BranchId}, CompanyId {request.CompanyId} ", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = "Tạo phòng ban thất bại";
        }

        return response;
    }

    public async Task<ApiResult<int>> CreatePositionAsync(CreatePositionRequest request)
    {
        var response = new ApiResult<int>()
        {
            Data = 0,
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (string.IsNullOrEmpty(request.Name))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập tên vị trí.";
            return response;
        }

        if (request.DepartmentId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "ID phòng ban không hợp lệ.";
            return response;
        }

        if (request.CompanyId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "ID công ty không hợp lệ.";
            return response;
        }

        try
        {
            var positionId = await _companyRepository.CreatePositionAsync(request.Name, request.DepartmentId, request.CompanyId);

            if (positionId > 0)
            {
                response.Data = positionId;
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = "Tạo vị trí thành công";
                return response;
            }

            response.Data = positionId;
            response.Code = ResponseResultEnum.Failed.Value();
            response.Message = "Tạo vị trí thất bại";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreatePositionAsync Exception Name {request.Name}, DepartmentId {request.DepartmentId}, CompanyId {request.CompanyId} ", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = "Tạo vị trí thất bại";
        }

        return response;
    }

    public async Task<ApiResult<List<DepartmentCreatedResult>>> CreateDepartmentInAllBranchesAsync(CreateDepartmentInAllBranchesRequest request)
    {
        var response = new ApiResult<List<DepartmentCreatedResult>>()
        {
            Data = new List<DepartmentCreatedResult>(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (request.Names == null || !request.Names.Any())
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Danh sách tên phòng ban không được rỗng.";
            return response;
        }

        if (request.CompanyId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "ID công ty không hợp lệ.";
            return response;
        }

        try
        {
            var departments = await _companyRepository.CreateDepartmentsInAllBranchesOptimizedAsync(request.CompanyId, request.Names);

            if (departments.Any())
            {
                response.Data = departments;
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = "Tạo phòng ban thành công";
                return response;
            }

            response.Code = ResponseResultEnum.Failed.Value();
            response.Message = "Tạo phòng ban thất bại";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartmentInAllBranchesAsync Exception Names {string.Join(", ", request.Names)}, CompanyId {request.CompanyId} ", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = "Tạo phòng ban thất bại";
        }

        return response;
    }

    public async Task<ApiResult<List<ListBusinessResponse>>> ListBusinessResponseAsync()
    {
        var response = new ApiResult<List<ListBusinessResponse>>()
        {
            Data = new List<ListBusinessResponse>(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        try
        {
            var departments = await _companyRepository.BusinessGetListAsync();

            if (departments.Any())
            {
                response.Data = departments.Select(d => new ListBusinessResponse
                {
                    Id = d.Id,
                    Value = "",
                    Name = d.Business,
                    Alias = d.Alias,
                    IndexNum = d.IndexNum ?? 0
                }).ToList();

                response.Code = ResponseResultEnum.Success.Value();
                response.Message = ResponseResultEnum.Success.Text();
                return response;
            }

            response.Code = ResponseResultEnum.NoData.Value();
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"ListBusinessResponseAsync Exception", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
        }

        return response;
    }
    
    public async Task<ApiResult<int>> UpdateUserAndShopNameAsync(UpdateInfoWhenSinupRequest request)
    {
        var response = new ApiResult<int>()
        {
            Data = 0,
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        try
        {
            var departments = await _companyRepository.UpdateInfoWhenSinup(request);
            response.Code = ResponseResultEnum.Success.Value();
            response.Message = ResponseResultEnum.Success.Text();
            response.Code = ResponseResultEnum.NoData.Value();
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"ListBusinessResponseAsync Exception", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = ResponseResultEnum.SystemError.Text();
        }

        return response;
    }
} 