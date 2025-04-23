using System.Numerics;
using AuthModule.DTOs;
using AuthModule.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Result;
using Shared.Utils;
using Shared.Enums;
using System.Threading.Tasks;
using Shared.Helpers;

namespace AuthModule.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// API đăng nhập hệ thống
    /// </summary>
    /// <param name="request">Thông tin đăng nhập</param>
    /// <returns>Token nếu đăng nhập thành công</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _authService.LoginAsync(request.Email, request.Password, ip, userAgent);

             return Ok(result);
        }
        catch (ArgumentException ex)
        {
            LoggerHelper.Warning($"Login Tham số không hợp lệ. Lỗi: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetEmployee ID: Exception.", ex);
            return StatusCode(500,
                new { message = "Đã xảy ra lỗi trong quá trình Login (-1)." });
        }      
    }


}