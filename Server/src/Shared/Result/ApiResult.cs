namespace Shared.Result;

public class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResult<T> Success(T data, string message = "")
    {
        return new ApiResult<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResult<T> Failure(string message, List<string>? errors = null)
    {
        return new ApiResult<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
} 