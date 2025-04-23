using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthModule.Middleware;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly int[] _roles;

    public AuthorizeAttribute(params int[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.Items["UserId"];
        var userRole = context.HttpContext.Items["Role"];

        if (userId == null)
        {
            // Không được xác thực
            context.Result = new JsonResult(new { message = "Unauthorized" }) 
            { 
                StatusCode = StatusCodes.Status401Unauthorized 
            };
            return;
        }

        if (_roles != null && _roles.Any() && userRole != null && !_roles.Contains((int)userRole))
        {
            // Role không được phép
            context.Result = new JsonResult(new { message = "Forbidden" }) 
            { 
                StatusCode = StatusCodes.Status403Forbidden 
            };
            return;
        }
    }
} 