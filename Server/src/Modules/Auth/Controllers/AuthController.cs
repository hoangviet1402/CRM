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
    [HttpPost("auth/set-password-new-user")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> CreatePass([FromBody] CreatePassRequest request)
    {
        try
        {
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _authService.CreatePassFornewEmployeeAsync(request.UserId, request.ShopId, request.Password, request.ComfirmPass);

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

            var result = await _authService.CreatePassFornewEmployeeAsync(request.UserId, request.ShopId, request.NewPass, request.OldPass);

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


    [HttpPost("signup/phone")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> Signup_phone([FromBody] SignupRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Stage))
            {
                return Ok(new ApiResult<AuthResponse>()
                {
                    Code = ResponseResultEnum.Failed.Value(),
                    Message = ResponseResultEnum.Failed.Text()
                });
            }
            switch (request.Stage.ToLower())
            {
                case "signin":
                    var resultSignin =   await _authService.SigninAsync(request, true);
                    return Ok(resultSignin);
                case "validate":
                    var resultValidate = await _authService.ValidateAccountAsync(request, true);
                    return Ok(resultValidate);
                case "signup":
                    var resultSignup =   await _authService.SignupAsync(request, true);
                    return Ok(resultSignup);
                default:
                    var resultSignup1 =  await _authService.SignupAsync(request, true);
                    return Ok(resultSignup1);
            }
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

    [HttpPost("signup/phone/update-fullname")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> UpdateFullname([FromBody] UpdateFullNameResquest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Phone))
            {
                return BadRequest(new { message = "Số điện thoại không được để trống." });
            }
            var result = await _authService.UpdateFullNameAsync(request.Phone, request.FullName, true);
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