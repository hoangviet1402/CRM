using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthModule.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Helpers;

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
            await AttachUserToContext(context, authRepository , token);

        await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, IAuthRepository authRepository , string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 minutes clock skew
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);

            // Lấy jti từ token
            var jwtToken = (JwtSecurityToken)validatedToken;
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            // Lấy thông tin claims
            var employeeId = principal.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            var companyId = principal.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            var accountId = principal.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (accountId != null && companyId != null && role != null && string.IsNullOrEmpty(jti) == false)
            {
                // Kiểm tra token trong database
                var storedToken = await authRepository.GetTokenInfo(int.Parse(accountId), int.Parse(companyId));
                if (storedToken != null && storedToken.IsActive  && storedToken.AccountIsActive && storedToken.CompanyIsActive && storedToken.JwtID.Equals(AESHelper.HashPassword(jti)))
                {

                    context.Items["AccountId"] = int.Parse(accountId);
                    context.Items["EmployeeId"] = int.Parse(employeeId ?? "0");
                    context.Items["CompanyId"] = int.Parse(companyId);
                    context.Items["JwtID"] = jti;
                    context.Items["Role"] = int.Parse(role);
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
