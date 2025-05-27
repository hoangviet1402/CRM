using AuthModule.Middleware;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;

namespace Company.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Tạo mới công ty
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request)
    {
        try
        {
            var result = await _companyService.CreateCompanyAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateCompany Exception.", ex);
            return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình tạo công ty." });
        }
    }

    /// <summary>
    /// Tạo mới chi nhánh
    /// </summary>
    [HttpPost("branch")]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request)
    {
        try
        {
            var result = await _companyService.CreateBranchAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateBranch Exception.", ex);
            return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình tạo chi nhánh." });
        }
    }

    /// <summary>
    /// Tạo nhiều chi nhánh cùng lúc
    /// </summary>
    [HttpPost("branches")]
    public async Task<IActionResult> CreateBranches([FromBody] CreateBranchesRequest request)
    {
        try
        {
            var result = await _companyService.CreateBranchesAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateBranches Exception.", ex);
            return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình tạo chi nhánh." });
        }
    }

    /// <summary>
    /// Tạo mới phòng ban
    /// </summary>
    [HttpPost("department")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        try
        {
            var result = await _companyService.CreateDepartmentAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartment Exception.", ex);
            return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình tạo phòng ban." });
        }
    }

    /// <summary>
    /// Tạo mới vị trí
    /// </summary>
    [HttpPost("position")]
    public async Task<IActionResult> CreatePosition([FromBody] CreatePositionRequest request)
    {
        try
        {
            var result = await _companyService.CreatePositionAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreatePosition Exception.", ex);
            return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình tạo vị trí." });
        }
    }

    /// <summary>
    /// Tạo phòng ban ở tất cả chi nhánh của công ty
    /// </summary>
    [HttpPost("department/all-branches")]
    public async Task<IActionResult> CreateDepartmentInAllBranches([FromBody] CreateDepartmentInAllBranchesRequest request)
    {
        try
        {
            var result = await _companyService.CreateDepartmentInAllBranchesAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartmentInAllBranches Exception.", ex);
            return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình tạo phòng ban." });
        }
    }
}