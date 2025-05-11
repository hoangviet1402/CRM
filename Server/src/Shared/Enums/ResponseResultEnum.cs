namespace Shared.Enums;

/// <summary>
/// Enum định nghĩa các loại kết quả response
/// </summary>
public enum ResponseResultEnum
{
    // General Results
    Success = 1,
    Failed = 2,
    PartialSuccess = 3,
    
    // Validation Results
    ValidationFailed = 10,
    InvalidInput = 11,
    InvalidState = 12,
    BusinessRuleViolation = 13,
    
    // Authentication Results
    Unauthorized = 20,
    TokenExpired = 21,
    InvalidCredentials = 22,
    AccountLocked = 23,
    SessionExpired = 24,
    InvalidToken = 25,
    InvalidPass = 26,


    // Authorization Results
    Forbidden = 30,
    InsufficientPermissions = 31,
    ResourceAccessDenied = 32,
    LicenseRequired = 33,
    InvalidSession = 34,
    InvalidRole = 35,

    // Resource Results
    NotFound = 40,
    AlreadyExists = 41,
    Deleted = 42,
    Modified = 43,
    Locked = 44,
    
    // Processing Results
    Processing = 50,
    Queued = 51,
    Completed = 52,
    Cancelled = 53,
    TimedOut = 54,
    
    // Data Results
    NoData = 60,
    PartialData = 61,
    InvalidData = 62,
    DataNotReady = 63,
    StaleData = 64,
    
    // File Operation Results
    FileNotFound = 70,
    FileAccessError = 71,
    FileUploadSuccess = 72,
    FileUploadFailed = 73,
    InvalidFileType = 74,
    
    // Integration Results
    ExternalServiceError = 80,
    NetworkError = 81,
    DatabaseError = 82,
    CacheError = 83,
    DependencyFailed = 84,
    
    // Batch Operation Results
    BatchSuccess = 90,
    BatchFailed = 91,
    BatchPartialSuccess = 92,
    BatchProcessing = 93,
    BatchCancelled = 94,
    
    // System Results
    SystemError = 100,
    MaintenanceMode = 101,
    ServiceUnavailable = 102,
    VersionMismatch = 103,
    ConfigurationError = 104,
    
    // Custom Business Results
    RequiresAction = 110,
    PendingApproval = 111,
    Rejected = 112,
    OnHold = 113,
    InProgress = 114,

    //Company
    CompanyLocked = 200,
    MoreThanOneCompany = 201,
    //Employees    
    EmployeesLocked = 300,
    EmployeesNeedSetPass = 301,

} 