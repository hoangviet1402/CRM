using AuthModule.Middleware;
using AuthModule.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Enums;
using Shared.Helpers;
using Shared.Result;
using Shared.Utils;
namespace Company.Controllers;

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
    /// Tạo nhiều chi nhánh cùng lúc
    /// </summary>
    [Authorize]
    [HttpPost("branch-add")]
    public async Task<IActionResult> CreateBranches([FromBody] List<CreateBranchesRequest> request)
    {
        try
        {
            var companyId =  HttpContext.GetCompanyId();
            var accountId = HttpContext.GetAccountId();
            if (companyId <= 0 || accountId <= 0)
            {
                return BadRequest(new { message = "Thông tin tài khoản hoặc công ty không hợp lệ." });
            }

            if (request == null || request.Count == 0)
            {
                return BadRequest(new { message = "Danh sách chi nhánh không được để trống." });
            }

            var result = await _companyService.CreateBranchesAsync(companyId, request);
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
    [Authorize]
    [HttpPost("department-add")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        try
        {
            var companyId =  HttpContext.GetCompanyId();
            var accountId = HttpContext.GetAccountId();
            if (companyId <= 0 || accountId <= 0)
            {
                return BadRequest(new { message = "Thông tin tài khoản hoặc công ty không hợp lệ." });
            }

            if (request == null || request.Name == null || request.Name.Count() == 0)
            {
                return BadRequest(new { message = "Danh sách phòng ban không được để trống." });
            }
            var result = await _companyService.CreateDepartmentAllBranchAsync(companyId,request);
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
    [HttpPost("position-add")]
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

    [Authorize]
    [HttpPost("element/list-business-field")]
    public async Task<IActionResult> listBusinessField()
    {
        try
        {
            var result = await _companyService.ListBusinessResponseAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartmentInAllBranches Exception.", ex);
            return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình lấy danh sách." });
        }
    }
  
    [Authorize]
    [HttpPost("update-user-and-shop-name")]
    public async Task<IActionResult> UpdateUserAndShopName(UpdateInfoWhenSinupRequest request)
    {
        try
        {           
            if (request == null)
            {
                return BadRequest(new { message = "Yêu cầu không hợp lệ." });
            }

            request.CompanyId =  HttpContext.GetCompanyId();
            request.AccountId = HttpContext.GetAccountId();
            if (request.AccountId <= 0 || request.CompanyId <= 0 || string.IsNullOrWhiteSpace(request.CompanyName))
            {
                return BadRequest(new { message = "Thông tin tài khoản hoặc công ty không hợp lệ." });
            }

            if (request.CompanyLatitude < -90 || request.CompanyLatitude > 90 || request.CompanyLongitude < -180 || request.CompanyLongitude > 180)
            {
                return BadRequest(new { message = "Vĩ độ hoặc kinh độ không hợp lệ." });
            }

            if (string.IsNullOrWhiteSpace(request.CompanyAddress))
            {
                return BadRequest(new { message = "Địa chỉ công ty không được để trống." });
            }

            if (string.IsNullOrWhiteSpace(request.Email) || ValidationHelper.IsValidEmail(request.Email) == false)
            {
                return BadRequest(new { message = "Email không hợp lệ." });
            }
            
            if (request.HearAbout == null || request.UsePurpose == null)
            {
                return BadRequest(new { message = "Thông tin về nguồn gốc và mục đích sử dụng không được để trống." });
            }

            var result = await _companyService.UpdateUserAndShopNameAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartmentInAllBranches Exception.", ex);
            return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình xử lý." });
        }
    }
}