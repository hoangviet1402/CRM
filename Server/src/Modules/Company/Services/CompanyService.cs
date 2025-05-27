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

    public async Task<ApiResult<int>> CreateBranchAsync(CreateBranchRequest request)
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
            response.Message = "Vui lòng nhập tên chi nhánh.";
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
            var branchId = await _companyRepository.CreateBranchAsync(request.Name, request.CompanyId);
            
            if (branchId > 0)
            {
                var result = await _companyRepository.UpdateCompanyStepAsync(CompanyStep.Branch.Value(), request.CompanyId);
                response.Data = branchId;
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = "Tạo chi nhánh thành công";
                return response;
            }

            response.Data = branchId;
            response.Code = ResponseResultEnum.Failed.Value();
            response.Message = "Tạo chi nhánh thất bại";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateBranchAsync Exception Name {request.Name}, CompanyId {request.CompanyId} ", ex);
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
                var result = await _companyRepository.UpdateCompanyStepAsync(CompanyStep.Department.Value(), request.CompanyId);
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

    public async Task<ApiResult<List<int>>> CreateBranchesAsync(CreateBranchesRequest request)
    {
        var response = new ApiResult<List<int>>()
        {
            Data = new List<int>(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        if (request.CompanyId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "ID công ty không hợp lệ.";
            return response;
        }

        if (request.BranchNames == null || !request.BranchNames.Any())
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Danh sách tên chi nhánh không được rỗng.";
            return response;
        }

        try
        {
            var branchIds = await _companyRepository.CreateBranchesOptimizedAsync(request.CompanyId, request.BranchNames);
            
            if (branchIds.Any())
            {
                response.Data = branchIds;
                response.Code = ResponseResultEnum.Success.Value();
                response.Message = "Tạo chi nhánh thành công";
                return response;
            }

            response.Code = ResponseResultEnum.Failed.Value();
            response.Message = "Tạo chi nhánh thất bại";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateBranchesAsync Exception CompanyId {request.CompanyId}, BranchNames {string.Join(", ", request.BranchNames)} ", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = "Tạo chi nhánh thất bại";
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
} 