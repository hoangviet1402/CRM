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
            Code = ResponseCodeEnum.SystemMaintenance.Value(),
            Message = ResponseCodeEnum.SystemMaintenance.Text()
        };

        if (string.IsNullOrEmpty(request.FullName))
        {
            throw new ArgumentException("Vui lòng nhập tên công ty.", nameof(request.FullName));
        }

        if (string.IsNullOrEmpty(request.Address))
        {
            throw new ArgumentException("Vui lòng nhập địa chỉ công ty.", nameof(request.Address));
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
} 