using AutoMapper;
using AutoMapper.Execution;
using EmployeeModule.DTOs;
using EmployeeModule.Entities;
using EmployeeModule.Repositories;
using Shared.Enums;
using Shared.Helpers;
using Shared.Result;
using Shared.Utils;

namespace EmployeeModule.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeEntity?> GetEmployeeByIdAsync(int id, int companyId)
    {
        // Validate input (có thể thêm FluentValidation ở đây)
        if (id <= 0)
            throw new ArgumentException("ID is required.", nameof(id));

        // Gọi stored procedure thông qua repository
        var newEmployeeId = await _employeeRepository.GetEmployeeById(
            id: id,
            companyId : companyId
        );

        return newEmployeeId;
    }

    public async Task<ApiResult<EmployeeCreate_ResultEntities>> CreateEmployeeAsync(string fullname, string phone, string email, string password,string employeeCode, int companyId, int role)
    {
        var response = new ApiResult<EmployeeCreate_ResultEntities>()
        {
            Data = new EmployeeCreate_ResultEntities(),
            Code = ResponseResultEnum.ServiceUnavailable.Value(),
            Message = ResponseResultEnum.ServiceUnavailable.Text()
        };

        #region Validate input 
        if (string.IsNullOrEmpty(fullname))
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng nhập họ tên.";
            return response;
        }

        if (companyId <= 0)
        {
            response.Code = ResponseResultEnum.InvalidInput.Value();
            response.Message = "Vui lòng chọn công ty của nhân viên.";
            return response;
        }
        #endregion

        try
        {
            if (string.IsNullOrEmpty(phone))
            {
                phone = Helper.GenerateUniqueNumber(11);
            }

            if (string.IsNullOrEmpty(email))
            {
                email = $"{phone}@mail.com";
            }

            if (role <= 0)
            {
                role = UserRole.Employees.Value();
            }

            var existingUser = await _employeeRepository.EmployeeRegister(fullname, employeeCode, phone, email, password, companyId, role);
            response.Data = existingUser;
            switch (existingUser.EmployeeAccountId)
            {
                case 0:
                    LoggerHelper.Warning($"Registration failed 0: email {email} already exists :fullname {fullname},phone {phone},password {password},companyId {companyId},role {role}");
                    response.Code = ResponseResultEnum.Failed.Value();
                    response.Message = "Chưa tạo được tài khoản do hệ thống bận vui lòng thử lại sau.";
                    break;
                default:
                    response.Code = ResponseResultEnum.Success.Value();
                    response.Message = $"tạo tài khoản {email} thành công";
                    break;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateEmployeeAsync Exception email {email}, fullname {fullname},phone {phone},password {password},companyId {companyId},role {role} ", ex);
            response.Code = ResponseResultEnum.SystemError.Value();
            response.Message = $"Có lỗi khi tạo tài khoản {email}";            
        }
        return response;
    }


}