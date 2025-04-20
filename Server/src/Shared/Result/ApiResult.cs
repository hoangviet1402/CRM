using Shared.Enums;
using Shared.Helpers;

namespace Shared.Result;

public class ApiResult<T>
{
    public int Code { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResult<T> Success(T data, string message = "")
    {
        return new ApiResult<T>
        {
            Code = ResponseCodeEnum.Success.Value(),
            Data = data,
            Message = message
        };
    }

    public static ApiResult<T> Failure(int code, string message, List<string>? errors = null)
    {
        return new ApiResult<T>
        {
            Code = code,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
} 