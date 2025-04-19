using Microsoft.AspNetCore.Http;
using System.Net;

namespace Shared.Utils;

public static class HttpContextExtensions
{
    /// <summary>
    /// Lấy IP address của client từ HttpContext
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>IP address dưới dạng string</returns>
    public static string GetClientIpAddress(this HttpContext context)
    {
        // Thứ tự ưu tiên:
        // 1. X-Forwarded-For header (nếu request đi qua proxy/load balancer)
        // 2. X-Real-IP header
        // 3. RemoteIpAddress từ connection

        string? ip = null;

        // Kiểm tra X-Forwarded-For header
        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            // X-Forwarded-For có thể chứa nhiều IP, lấy IP đầu tiên
            ip = forwardedHeader.Split(',').FirstOrDefault()?.Trim();
        }

        // Kiểm tra X-Real-IP header
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }

        // Lấy IP từ connection
        if (string.IsNullOrEmpty(ip) && context.Connection.RemoteIpAddress != null)
        {
            ip = context.Connection.RemoteIpAddress.ToString();
        }

        // Nếu là IPv6 loopback, chuyển thành IPv4 loopback
        if (ip == "::1")
        {
            ip = "127.0.0.1";
        }

        // Kiểm tra tính hợp lệ của IP
        if (!string.IsNullOrEmpty(ip) && IPAddress.TryParse(ip, out _))
        {
            return ip;
        }

        return "0.0.0.0"; // IP không hợp lệ hoặc không tìm thấy
    }

    /// <summary>
    /// Kiểm tra xem request có phải từ localhost không
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>true nếu request từ localhost</returns>
    public static bool IsLocalRequest(this HttpContext context)
    {
        var ip = context.GetClientIpAddress();
        return ip == "127.0.0.1" || ip == "::1" || ip == "localhost";
    }
} 