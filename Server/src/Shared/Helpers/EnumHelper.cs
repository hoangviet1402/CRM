using System.ComponentModel;
using System.Reflection;
using Shared.Enums;

namespace Shared.Helpers;

/// <summary>
/// Helper class để xử lý các giá trị và mô tả của enums
/// </summary>
public static class EnumHelper
{
    /// <summary>
    /// Lấy giá trị số của enum
    /// </summary>
    public static int Value(this Enum enumValue)
    {
        return Convert.ToInt32(enumValue);
    }

    /// <summary>
    /// Lấy mô tả tiếng Việt của enum
    /// </summary>
    public static string Text(this Enum enumValue)
    {
        return enumValue switch
        {
            ResponseResultEnum result => GetResponseResultText(result),
            _ => enumValue.ToString()
        };
    }


    /// <summary>
    /// Lấy mô tả tiếng Việt cho ResponseResultEnum
    /// </summary>
    private static string GetResponseResultText(ResponseResultEnum result)
    {
        return result switch
        {
            // General Results
            ResponseResultEnum.Success => "Thành công",
            ResponseResultEnum.Failed => "Thất bại",
            ResponseResultEnum.PartialSuccess => "Thành công một phần",
            
            // Validation Results
            ResponseResultEnum.ValidationFailed => "Xác thực thất bại",
            ResponseResultEnum.InvalidInput => "Đầu vào không hợp lệ",
            ResponseResultEnum.InvalidState => "Trạng thái không hợp lệ",
            ResponseResultEnum.BusinessRuleViolation => "Vi phạm quy tắc nghiệp vụ",
            
            // Authentication Results
            ResponseResultEnum.Unauthorized => "Chưa xác thực",
            ResponseResultEnum.TokenExpired => "Token hết hạn",
            ResponseResultEnum.InvalidCredentials => "Thông tin đăng nhập không hợp lệ",
            ResponseResultEnum.AccountLocked => "Tài khoản bị khóa",
            ResponseResultEnum.SessionExpired => "Phiên đã hết hạn",
            
            // Authorization Results
            ResponseResultEnum.Forbidden => "Không có quyền truy cập",
            ResponseResultEnum.InsufficientPermissions => "Không đủ quyền",
            ResponseResultEnum.ResourceAccessDenied => "Từ chối truy cập tài nguyên",
            ResponseResultEnum.LicenseRequired => "Yêu cầu giấy phép",
            
            // Resource Results
            ResponseResultEnum.NotFound => "Không tìm thấy",
            ResponseResultEnum.AlreadyExists => "Đã tồn tại",
            ResponseResultEnum.Deleted => "Đã xóa",
            ResponseResultEnum.Modified => "Đã sửa đổi",
            ResponseResultEnum.Locked => "Đã khóa",
            
            // Processing Results
            ResponseResultEnum.Processing => "Đang xử lý",
            ResponseResultEnum.Queued => "Đang trong hàng đợi",
            ResponseResultEnum.Completed => "Đã hoàn thành",
            ResponseResultEnum.Cancelled => "Đã hủy",
            ResponseResultEnum.TimedOut => "Hết thời gian",
            
            // Data Results
            ResponseResultEnum.NoData => "Không có dữ liệu",
            ResponseResultEnum.PartialData => "Dữ liệu một phần",
            ResponseResultEnum.InvalidData => "Dữ liệu không hợp lệ",
            ResponseResultEnum.DataNotReady => "Dữ liệu chưa sẵn sàng",
            ResponseResultEnum.StaleData => "Dữ liệu cũ",
            
            // File Operation Results
            ResponseResultEnum.FileNotFound => "Không tìm thấy tệp",
            ResponseResultEnum.FileAccessError => "Lỗi truy cập tệp",
            ResponseResultEnum.FileUploadSuccess => "Tải lên tệp thành công",
            ResponseResultEnum.FileUploadFailed => "Tải lên tệp thất bại",
            ResponseResultEnum.InvalidFileType => "Loại tệp không hợp lệ",
            
            // Integration Results
            ResponseResultEnum.ExternalServiceError => "Lỗi dịch vụ bên ngoài",
            ResponseResultEnum.NetworkError => "Lỗi mạng",
            ResponseResultEnum.DatabaseError => "Lỗi cơ sở dữ liệu",
            ResponseResultEnum.CacheError => "Lỗi bộ nhớ đệm",
            ResponseResultEnum.DependencyFailed => "Lỗi phụ thuộc",
            
            // Batch Operation Results
            ResponseResultEnum.BatchSuccess => "Xử lý hàng loạt thành công",
            ResponseResultEnum.BatchFailed => "Xử lý hàng loạt thất bại",
            ResponseResultEnum.BatchPartialSuccess => "Xử lý hàng loạt thành công một phần",
            ResponseResultEnum.BatchProcessing => "Đang xử lý hàng loạt",
            ResponseResultEnum.BatchCancelled => "Đã hủy xử lý hàng loạt",
            
            // System Results
            ResponseResultEnum.SystemError => "Lỗi hệ thống",
            ResponseResultEnum.MaintenanceMode => "Chế độ bảo trì",
            ResponseResultEnum.ServiceUnavailable => "Dịch vụ không khả dụng",
            ResponseResultEnum.VersionMismatch => "Không khớp phiên bản",
            ResponseResultEnum.ConfigurationError => "Lỗi cấu hình",
            
            // Custom Business Results
            ResponseResultEnum.RequiresAction => "Yêu cầu hành động",
            ResponseResultEnum.PendingApproval => "Đang chờ phê duyệt",
            ResponseResultEnum.Rejected => "Đã từ chối",
            ResponseResultEnum.OnHold => "Đang tạm giữ",
            ResponseResultEnum.InProgress => "Đang tiến hành",
            
            _ => result.ToString()
        };
    }
} 