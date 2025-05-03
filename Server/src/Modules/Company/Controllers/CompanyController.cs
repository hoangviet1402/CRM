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
        catch (ArgumentException ex)
        {
            LoggerHelper.Warning($"CreateCompany Tham số không hợp lệ. ID: {request.FullName}. Lỗi: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateCompany Exception.", ex);
            return StatusCode(500,
                new { message = "Đã xảy ra lỗi trong quá trình lấy thông tin nhân viên." });
        }
    }
}