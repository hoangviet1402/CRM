namespace Shared.Enums;

/// <summary>
/// Enum định nghĩa các mã response chuẩn cho API
/// </summary>
public enum ResponseCodeEnum
{
    // Success Codes (2xx)
    Success = 200,
    Created = 201,
    Accepted = 202,
    NoContent = 204,
    
    // Client Error Codes (4xx)
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    Conflict = 409,
    ValidationError = 422,
    TooManyRequests = 429,
    
    // Server Error Codes (5xx)
    InternalServerError = 500,
    NotImplemented = 501,
    BadGateway = 502,
    ServiceUnavailable = 503,
    
    // Custom Business Codes (1xxx)
    ValidationFailed = 1001,
    DuplicateEntry = 1002,
    InvalidCredentials = 1003,
    ExpiredToken = 1004,
    InsufficientPermissions = 1005,
    ResourceLocked = 1006,
    DependencyError = 1007,
    BusinessRuleViolation = 1008,
    
    // Integration Error Codes (2xxx)
    ExternalServiceError = 2001,
    DatabaseError = 2002,
    CacheError = 2003,
    MessageQueueError = 2004,
    FileSystemError = 2005,
    NetworkError = 2006,
    
    // Data Error Codes (3xxx)
    DataNotFound = 3001,
    DataValidationFailed = 3002,
    DataCorrupted = 3003,
    DataTypeError = 3004,
    DataConflict = 3005,
    DataCreateFail = 3006,

    // Security Error Codes (4xxx)
    SecurityViolation = 4001,
    InvalidToken = 4002,
    AccountLocked = 4003,
    PasswordExpired = 4004,
    InvalidSession = 4005,
    InvalidRole = 4006,

    // File Operation Codes (5xxx)
    FileNotFound = 5001,
    FileAccessDenied = 5002,
    FileTooLarge = 5003,
    InvalidFileType = 5004,
    FileUploadFailed = 5005,
    
    // Batch Operation Codes (6xxx)
    BatchProcessingFailed = 6001,
    PartialSuccess = 6002,
    BatchValidationFailed = 6003,
    BatchSizeLimitExceeded = 6004,

    SystemError = 7001,
    SystemMaintenance = 7002
} 