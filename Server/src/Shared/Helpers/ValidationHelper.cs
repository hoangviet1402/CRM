using System.Text.RegularExpressions;

namespace Shared.Helpers;

/// <summary>
/// Helper class để kiểm tra tính hợp lệ của email và số điện thoại
/// </summary>
public static class ValidationHelper
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Ví dụ: kiểm tra số điện thoại Việt Nam (bắt đầu bằng 0, 84, +84, 10-11 số)
    private static readonly Regex PhoneRegex = new Regex(
        @"^(0|\+?84)(\d{9,10})$",
        RegexOptions.Compiled);

    /// <summary>
    /// Kiểm tra chuỗi có phải là email hợp lệ không
    /// </summary>
    public static bool IsValidEmail(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        return EmailRegex.IsMatch(input.Trim());
    }

    /// <summary>
    /// Kiểm tra chuỗi có phải là số điện thoại hợp lệ không
    /// </summary>
    public static bool IsValidPhone(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var phone = input.Trim().Replace(" ", "").Replace("-", "");
        return PhoneRegex.IsMatch(phone);
    }
}