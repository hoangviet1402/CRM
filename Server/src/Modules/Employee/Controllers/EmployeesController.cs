using System.Numerics;
using EmployeeModule.DTOs;
using EmployeeModule.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;

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
                return NotFound(new { message = $"Employee with ID {id} not found." });

            return Ok(employee);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log error here if needed
            return StatusCode(500,
                new { message = "An error occurred while retrieving the employee." });
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
            var employeeId = await _employeeService.CreateEmployeeSimple(request.FullName, request.EmployeeCode, request.Email, request.Phone);
            
            // Trả về 201 Created với Id của employee mới
            return CreatedAtAction(
                nameof(GetEmployee), // Tên action method để lấy chi tiết employee
                new { id = employeeId }, // Route values
                employeeId // Response body
            );
        }
        catch (ArgumentException ex)
        {
            // Xử lý validation errors
            ModelState.AddModelError(ex.ParamName ?? "Error", ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (Exception ex)
        {
            // Log error here if needed
            return StatusCode(500, // Internal Server Error
                new { message = "An error occurred while creating the employee." });
        }
    }
} 