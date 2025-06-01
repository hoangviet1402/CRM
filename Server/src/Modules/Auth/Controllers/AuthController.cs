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
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var isUsePhone = true;
            if (string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.PhoneCode))
            {
                return BadRequest(new { message = "Số điện thoại không được để trống." });
            }

            if (ValidationHelper.IsValidPhone($"{request.PhoneCode}{request.Phone}") == false)
            {
                return BadRequest(new { message = "Số điện thoại không hợp lệ." });
            }

            if (string.IsNullOrEmpty(request.Stage))
            {
                return BadRequest(new { message = "Thông tin không hợp lệ." });
            }
            var validateRequest = new ValidateAccountRequest()
            {
                PhoneCode = request.PhoneCode,
                Phone = request.Phone,
                Mail = request.Mail
            };
            switch (request.Stage.ToLower())
            {
                case "validate":
                    var resultValidate = await _authService.ValidateAccountAsync(validateRequest, isUsePhone);
                    if (resultValidate.Code == ResponseResultEnum.Success.Value())
                    {
                        if (resultValidate.Data != null && resultValidate.Data.AccountId != null)
                        {
                            var dataAlter = await _authService.GetDataAlterAsync(
                                resultValidate.Data.AccountId.Value,
                                isUsePhone,
                                $"{request.PhoneCode}{request.Phone}",
                                new List<string>() { "phone" });
                            return Ok(dataAlter);
                        }
                        return Ok(resultValidate);
                    }
                    return Ok(resultValidate);
                case "signup":
                    var result = await _authService.UpdateFullNameSigupAsync(
                        request.Phone,
                        request.Mail ?? string.Empty,
                        request.Fullname ?? string.Empty,
                        isUsePhone,
                        ip,
                        imie: userAgent);
                    return Ok(result);
                default:
                    return BadRequest(new { message = "Thông tin không hợp lệ." });
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

    [HttpPost("signin-v2")]
    [ProducesResponseType(typeof(ApiResult<AuthResponse>), 200)]
    public async Task<IActionResult> Signin([FromBody] SigninRequest request)
    {
        try
        {
            var ip = HttpContextExtensions.GetClientIpAddress(HttpContext);
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            var isUsePhone = request.Provider?.ToLower() == "phone";
            if (isUsePhone == true)
            {
                if (string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.PhoneCode))
                {
                    return BadRequest(new { message = "Số điện thoại không được để trống." });
                }

                if (ValidationHelper.IsValidPhone($"{request.PhoneCode}{request.Phone}") == false)
                {
                    return BadRequest(new { message = "Số điện thoại không hợp lệ." });
                }
            }
            else
            {
                if (string.IsNullOrEmpty(request.Mail))
                {
                    return BadRequest(new { message = "Email không được để trống." });
                }

                if (ValidationHelper.IsValidEmail(request.Mail) == false)
                {
                    return BadRequest(new { message = "Email không hợp lệ." });
                }
            }
            if (string.IsNullOrEmpty(request.Stage))
            {
                return BadRequest(new { message = "Thông tin không hợp lệ." });
            }

            var validateRequest = new ValidateAccountRequest()
            {
                PhoneCode = request.PhoneCode,
                Phone = request.Phone,
                Mail = request.Mail
            };
            switch (request.Stage.ToLower())
            {
                case "validate":
                    var resultValidate = await _authService.ValidateAccountAsync(validateRequest, isUsePhone);
                    if (resultValidate.Code == ResponseResultEnum.Success.Value())
                    {
                        if (resultValidate.Data != null && resultValidate.Data.AccountId != null)
                        {
                            var dataAlter = await _authService.GetDataAlterAsync(
                                resultValidate.Data.AccountId.Value,
                                isUsePhone,
                                $"{request.PhoneCode}{request.Phone}",
                                new List<string>() { "phone" });
                            return Ok(dataAlter);
                        }
                        return Ok(resultValidate);
                    }
                    return Ok(resultValidate);
                case "signin":
                    var result = await _authService.SigninAsync(request,isUsePhone,ip, userAgent);
                    return Ok(result);
                default:
                    return BadRequest(new { message = "Thông tin không hợp lệ." });
            }
        }
        catch (ArgumentException ex)
        {
            LoggerHelper.Warning($"Login Tham số không hợp lệ. Lỗi: {ex.Message}");
            return BadRequest(new { message = "Tham số không hợp lệ" });
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetEmployee ID: Exception.", ex);
            return StatusCode(500,
                new { message = "Đã xảy ra lỗi trong quá trình xử lý (-1)." });
        }
    }
}