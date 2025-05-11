using System.Net;
using System;
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
    //Lưu ý:
    //[Authorize] ở controller level là cấu hình mặc định cho tất cả actions 
    //Có thể override bằng cách thêm[Authorize] với tham số khác ở action level vd [Authorize((int)UserRole.SystemAdmin, (int)UserRole.BranchManager)] or [Authorize(1,2)] or [Authorize()]
    //Có thể sử dụng[AllowAnonymous] để cho phép truy cập công khai
    //Authorization ở action level sẽ override authorization ở controller level
    // **** Request -> Authentication Middleware -> Authorization Pipeline -> OnAuthorization -> Controller Action
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Convert EmployeeId to int
        var accountId = TryConvertToInt(context.HttpContext.Items["AccountId"]);
        var employeeId = TryConvertToInt(context.HttpContext.Items["EmployeeId"]);
        var companyId = TryConvertToInt(context.HttpContext.Items["CompanyId"]);
        var userRole = TryConvertToInt(context.HttpContext.Items["Role"]);

        if (accountId <= 0 || companyId <= 0 || userRole <= 0)
        {
            // Không được xác thực
            context.Result = new JsonResult(new { message = "Unauthorized" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        if (_roles != null && _roles.Any() && !_roles.Contains(userRole))
        {
            // Role không được phép
            context.Result = new JsonResult(new { message = "Forbidden" }) 
            { 
                StatusCode = StatusCodes.Status403Forbidden 
            };
            return;
        }
    }

    private int TryConvertToInt(object? value)
    {
        if (value == null)
            return 0;

        return int.TryParse(value.ToString(), out int result) ? result : 0;
    }
} 