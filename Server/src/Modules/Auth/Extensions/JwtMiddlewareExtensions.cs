using AuthModule.Middleware;
using Microsoft.AspNetCore.Builder;

namespace AuthModule.Extensions;

public static class JwtMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }
} 