using AuthModule.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Enums;
using Shared.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context, IAuthRepository authRepository)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
            AttachUserToContext(context, authRepository , token);

        await _next(context);
    }

    private async void AttachUserToContext(HttpContext context, IAuthRepository authRepository , string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);

            // Lấy thông tin claims
            var employeeId = principal.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            var companyId = principal.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            var accessToken = principal.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;
            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (employeeId != null && companyId != null && role != null && string.IsNullOrEmpty(accessToken) == false)
            {
                // Kiểm tra token trong database
                var storedToken = await authRepository.GetTokenInfo(int.Parse(employeeId));
                if (storedToken != null && storedToken.EmployeeIsActive && storedToken.CompanyIsActive && storedToken.AccessToken.Equals(AESHelper.HashPassword(accessToken)))
                {
                    context.Items["EmployeeId"] = int.Parse(employeeId);
                    context.Items["CompanyId"] = int.Parse(companyId);
                    context.Items["Role"] = role;
                }
                
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Warning($"[JWT ERROR] : {ex.Message}");
            // Không làm gì, tiếp tục request sẽ bị chặn ở tầng controller nếu không có context.Items
        }
    }
}
