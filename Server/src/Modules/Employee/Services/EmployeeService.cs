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


    public async Task<ApiResult<int>> CreateEmployeeAsync(string fullname, string phone, string email, string password,string employeeCode, int companyId, int role)
    {
        var response = new ApiResult<int>()
        {
            Data = 0,
            Code = ResponseCodeEnum.SystemMaintenance.Value(),
            Message = ResponseCodeEnum.SystemMaintenance.Text()
        };

        #region Validate input 
        if (string.IsNullOrWhiteSpace(fullname))
        {
            throw new ArgumentException("Vui lòng nhập họ tên.", nameof(fullname));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("Vui lòng nhập phone.", nameof(phone));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Vui lòng nhập email.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Vui lòng nhập mật khẩu.", nameof(password));
        }

        if (companyId <= 0)
        {
            throw new ArgumentException("Vui lòng chọn công ty của nhân viên.", nameof(companyId));
        }

        if (role <= 0)
        {
            throw new ArgumentException("Vui lòng phân quyền cho nhân viên này.", nameof(role));
        }

        #endregion

        try
        {

            var existingUser = await _employeeRepository.EmployeeRegister(fullname, phone, email, password, companyId, role);
            response.Data = existingUser;
            switch (existingUser)
            {
                case -1:
                    LoggerHelper.Warning($"Registration failed -1: email {email} already exists :fullname {fullname},phone {phone},password {password},companyId {companyId},role {role}");
                    response.Code = ResponseResultEnum.AlreadyExists.Value();
                    response.Message = "Tài khoản email này đã tồn tại trong công ty.";
                    break;
                case -2:
                    LoggerHelper.Warning($"Registration failed -2: email {email} already exists :fullname {fullname},phone {phone},password {password},companyId {companyId},role {role}");
                    response.Code = ResponseResultEnum.NoData.Value();
                    response.Message = "Công ty được chọn không tồn tại hoặc đã bị khóa.";
                    break;
                case -3:
                    LoggerHelper.Warning($"Registration failed -3: email {email} already exists :fullname {fullname},phone {phone},password {password},companyId {companyId},role {role}");
                    response.Code = ResponseResultEnum.NoData.Value();
                    response.Message = "Công ty được chọn chưa tạo mã định danh cho nhân viên, vui lòng nhập tay.";
                    break;
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