using Microsoft.AspNetCore.Http;
using Shared.Enums;

namespace AuthModule.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Lấy EmployeeId từ context
    /// </summary>
    public static int? GetEmployeeId(this HttpContext context)
    {
        return context.Items["EmployeeId"] as int?;
    }

    /// <summary>
    /// Lấy CompanyId từ context
    /// </summary>
    public static int? GetCompanyId(this HttpContext context)
    {
        return context.Items["CompanyId"] as int?;
    }

    /// <summary>
    /// Lấy Role từ context
    /// </summary>
    public static int? GetRole(this HttpContext context)
    {
        return context.Items["Role"] as int?;
    }

    /// <summary>
    /// Kiểm tra user có role được chỉ định không
    /// </summary>
    public static bool HasRole(this HttpContext context, int role)
    {
        var userRole = context.GetRole();
        return userRole.HasValue && userRole.Value == role;
    }

    /// <summary>
    /// Kiểm tra user có phải là SystemAdmin không
    /// </summary>
    public static bool IsSystemAdmin(this HttpContext context)
    {
        return context.HasRole((int)UserRole.SystemAdmin);
    }

    /// <summary>
    /// Kiểm tra user có phải là BranchManager không
    /// </summary>
    public static bool IsBranchManager(this HttpContext context)
    {
        return context.HasRole((int)UserRole.BranchManager);
    }
} 