using System.Numerics;
using EmployeeModule.DTOs;
using EmployeeModule.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;
using Shared.Helpers;
using Shared.Enums;

namespace EmployeeModule.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Get employee by id
    /// </summary>
    /// <param name="id">Employee id</param>
    /// <returns>Employee information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmployeeDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetEmployee(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = $"Không tìm thấy nhân viên với ID {id}." });

            return Ok(employee);
        }
        catch (ArgumentException ex)
        {
            LoggerHelper.Warning($"GetEmployee Tham số không hợp lệ. ID: {id}. Lỗi: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetEmployee ID: {id}.Exception.", ex);
            return StatusCode(500,
                new { message = "Đã xảy ra lỗi trong quá trình lấy thông tin nhân viên." });
        }
    }

    /// <summary>
    /// Create a new employee
    /// </summary>
    /// <param name="request">Employee information</param>
    /// <returns>Id of the newly created employee</returns>
    [HttpPost]
    [ProducesResponseType(201)] // Created
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)] // Bad Request
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        try
        {
            var data = await _employeeService.CreateEmployeeSimple(request.FullName, request.EmployeeCode, request.Email, request.Phone);
            return Ok(data);
            // Trả về 201 Created với Id của employee mới
            //return CreatedAtAction(
            //    nameof(GetEmployee), // Tên action method để lấy chi tiết employee
            //    new { id = data.Data.EmployeeId }, // Route values
            //    data // Response body
            //);
        }
        catch (ArgumentException ex)
        {
            LoggerHelper.Error($"GetEmployee ArgumentException.", ex);
            ModelState.AddModelError(ex.ParamName ?? "Error", ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetEmployee Exception.", ex);
            return StatusCode(200,new { message = "Đã xảy ra lỗi trong quá trình tạo nhân viên mới." });
        }
    }
} 