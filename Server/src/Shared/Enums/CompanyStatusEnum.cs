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

public enum SetupStep
{
    ONBOARDING_CREATE_BRANCH = 1,
    ONBOARDING_CREATE_DEPARTMENT = 2,
    ONBOARDING_CREATE_POSITION = 3,
    ONBOARDING_CREATE_EMPLOYEE = 4,
    ONBOARDING_CREATE_SHIFT = 5 
}
