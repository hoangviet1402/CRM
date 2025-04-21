using AutoMapper;
using EmployeeModule.DTOs;
using EmployeeModule.Entities;
using EmployeeModule.Repositories;
using Shared.Enums;
using Shared.Helpers;
using Shared.Result;

namespace EmployeeModule.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeEntity?> GetEmployeeByIdAsync(int id)
    {
        // Validate input (có thể thêm FluentValidation ở đây)
        if (id <= 0)
            throw new ArgumentException("ID is required.", nameof(id));

      
        // Gọi stored procedure thông qua repository
        var newEmployeeId = await _employeeRepository.GetEmployeeById(
            id: id
        );

        return newEmployeeId;
    }

    public async Task<ApiResult<CreateEmployeeResponse>> CreateEmployeeSimple(string fullName, string employeeCode, string email, string phone)
    {
        var response = new ApiResult<CreateEmployeeResponse>()
        {
            Data = new CreateEmployeeResponse(),
            Code = ResponseCodeEnum.SystemMaintenance.Value(),
            Message = ResponseCodeEnum.SystemMaintenance.Text()
        };

        // Validate input (có thể thêm FluentValidation ở đây)
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name is required.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(employeeCode))
        {
            throw new ArgumentException("Employee code is required.", nameof(employeeCode));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Phone is required.", nameof(phone));
        }

        try
        {
            // Gọi stored procedure thông qua repository
            response.Data = await _employeeRepository.CreateEmployeeSimple(
                fullName: fullName,
                employeeCode: employeeCode,
                email: email,
                phone: phone
            );
            response.Code = response.Data != null && response.Data.EmployeeId > 0 ? ResponseCodeEnum.Success.Value() : ResponseCodeEnum.DataCreateFail.Value();
            response.Message = response.Data != null && response.Data.EmployeeId > 0 ? "Thêm nhân viên thành công" : "Thêm nhân viên thất bại";
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateEmployeeSimple ex.", ex);
        }
        return response;
    }
} 