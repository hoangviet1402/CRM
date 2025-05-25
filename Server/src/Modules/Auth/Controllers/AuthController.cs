using AuthModule.DTOs;
using AuthModule.Extensions;
using AuthModule.Middleware;
using AuthModule.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Helpers;
using Shared.Result;
using Shared.Utils;

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

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();            
            var resultUser= await _authService.LoginAsync(request.AccountName, request.IsUsePhone, request.Password, ip, userAgent);           
             return Ok(resultUser);
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

    [Authorize]
    [HttpPost("refreshtoken")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var companyId = HttpContext.GetCompanyId();
            var accountId = HttpContext.GetAccountId();
            var jwtID = HttpContext.GetJwtID();

            var result = await _authService.RefreshTokenAsync(refreshToken, jwtID, accountId, companyId, ip, userAgent);

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

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        try
        {
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var companyId = HttpContext.GetCompanyId();
            var accountId = HttpContext.GetAccountId();

            var result = await _authService.LogoutAsync(refreshToken, accountId, companyId, ip, userAgent);

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

    [Authorize]
    [HttpPost("createpass")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> CreatePass([FromBody] CreatePassRequest request)
    {
        try
        {
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _authService.CreatePassFornewEmployeeAsync(request.EmployeeAccountMapId, request.NewPass, request.ComfirmPass);

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

    [Authorize]
    [HttpPost("changepass")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> ChangePass([FromBody] ChangePassRequest request)
    {
        try
        {
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _authService.CreatePassFornewEmployeeAsync(request.EmployeeAccountMapId, request.NewPass, request.OldPass);

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


    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> RegisterAccount([FromBody] LoginRequest request)
    {
        try
        {
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var resultUser = await _authService.LoginAsync(request.AccountName, request.IsUsePhone, request.Password, ip, userAgent);
            if(resultUser.Code == ResponseResultEnum.AccountNotExist.Value())
            {
                var resultNewUser = await _authService.RegisterAccount(request.AccountName, "" , request.IsUsePhone);
                return Ok(resultNewUser);
            }
            return Ok(resultUser);
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