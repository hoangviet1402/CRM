namespace Shared.Enums;

/// <summary>
/// Enum định nghĩa các trạng thái của công ty
/// </summary>
public enum CompanyStatusEnum
{
    /// <summary>
    /// Đang hoạt động
    /// </summary>
    Active = 1,

    /// <summary>
    /// Đã ngừng hoạt động
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Đã bị khóa
    /// </summary>
    Locked = 3,

    
}

public enum CompanyStep
{
    /// <summary>
    /// Đang hoạt động
    /// </summary>
    Begin = 0,

    /// <summary>
    /// Đã ngừng hoạt động
    /// </summary>
    Branch = 1,

    /// <summary>
    /// Đã bị khóa
    /// </summary>
    Department = 2,


}