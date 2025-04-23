using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AuthModule.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Constants;

namespace AuthModule.Middleware;

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

        if (token != null)
            await AttachUserToContext(context, authRepository, token);

        await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, IAuthRepository authRepository, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration[AppConstants.JWT_SECRET_KEY]);
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // Kiểm tra token trong database
            var storedToken = await authRepository.GetAccountTokenByUserId(userId);
            if (storedToken != null && storedToken.AccessToken == token && storedToken.Expires > DateTime.UtcNow)
            {
                // Attach user to context on successful jwt validation
                context.Items["UserId"] = userId;
                context.Items["CompanyId"] = int.Parse(jwtToken.Claims.First(x => x.Type == "companyId").Value);
                context.Items["Role"] = int.Parse(jwtToken.Claims.First(x => x.Type == "role").Value);
            }
        }
        catch
        {
            // Không làm gì - nếu xác thực JWT thất bại thì request sẽ tiếp tục 
            // mà không gắn user vào context
        }
    }
} 