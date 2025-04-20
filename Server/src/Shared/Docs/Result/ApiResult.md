# ApiResult

Class cơ sở để định nghĩa format response chuẩn cho API.

## Overview

`ApiResult<T>` là một generic class cung cấp cấu trúc response chuẩn cho API, bao gồm:
- Status code
- Success flag
- Message
- Data payload
- Error details (nếu có)

## Cấu trúc Cơ Bản

```csharp
public class ApiResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public int StatusCode { get; set; }
    public List<string> Errors { get; set; }
}
```

## Kế Thừa và Mở Rộng

### 1. Kế thừa trực tiếp

```csharp
// Kế thừa và thêm fields
public class PaginatedApiResult<T> : ApiResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}

// Sử dụng
var result = new PaginatedApiResult<List<Product>>
{
    Success = true,
    Data = products,
    Page = 1,
    PageSize = 10,
    TotalPages = 5,
    TotalItems = 50
};
```

### 2. Kế thừa với type cụ thể

```csharp
// Kế thừa với type cố định
public class UserApiResult : ApiResult<UserDto>
{
    public DateTime LastLoginTime { get; set; }
    public bool IsOnline { get; set; }
    
    public UserApiResult(UserDto user)
    {
        Success = true;
        Data = user;
    }
}

// Sử dụng
var result = new UserApiResult(userDto)
{
    LastLoginTime = DateTime.UtcNow,
    IsOnline = true
};
```

### 3. Kế thừa với multiple types

```csharp
// Base class cho response có nhiều data types
public class MultiDataApiResult<T1, T2> : ApiResult<T1>
{
    public T2 AdditionalData { get; set; }
}

// Sử dụng
var result = new MultiDataApiResult<OrderDto, PaymentDto>
{
    Success = true,
    Data = orderDto,
    AdditionalData = paymentDto
};
```

### 4. Specialized Results

```csharp
// Result cho validation errors
public class ValidationApiResult : ApiResult<Dictionary<string, string[]>>
{
    public ValidationApiResult(ModelStateDictionary modelState)
    {
        Success = false;
        StatusCode = 400;
        Message = "Validation failed";
        Data = modelState.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
        );
    }
}

// Result cho file operations
public class FileApiResult : ApiResult<byte[]>
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
}
```

## Factory Methods

```csharp
public static class ApiResult
{
    public static ApiResult<T> Success<T>(T data, string message = "Success")
    {
        return new ApiResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = 200
        };
    }

    public static ApiResult<T> Error<T>(string message, int statusCode = 400)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode
        };
    }
}
```

## Usage Examples

### 1. Basic Usage

```csharp
[HttpGet]
public async Task<ApiResult<List<Product>>> GetProducts()
{
    var products = await _productService.GetAllAsync();
    return ApiResult.Success(products);
}
```

### 2. Paginated Result

```csharp
[HttpGet]
public async Task<PaginatedApiResult<List<Product>>> GetProducts([FromQuery] int page = 1)
{
    var (products, total) = await _productService.GetPagedAsync(page);
    
    return new PaginatedApiResult<List<Product>>
    {
        Success = true,
        Data = products,
        Page = page,
        PageSize = 10,
        TotalItems = total,
        TotalPages = (int)Math.Ceiling(total / 10.0)
    };
}
```

### 3. Validation Result

```csharp
[HttpPost]
public async Task<ApiResult<ProductDto>> CreateProduct(CreateProductRequest request)
{
    if (!ModelState.IsValid)
    {
        return new ValidationApiResult(ModelState);
    }
    
    var product = await _productService.CreateAsync(request);
    return ApiResult.Success(product, "Product created successfully");
}
```

### 4. File Result

```csharp
[HttpGet("download/{id}")]
public async Task<FileApiResult> DownloadFile(int id)
{
    var file = await _fileService.GetFileAsync(id);
    
    return new FileApiResult
    {
        Success = true,
        Data = file.Content,
        FileName = file.Name,
        ContentType = file.ContentType,
        FileSize = file.Size
    };
}
```

## Best Practices

1. **Consistency in Response Format**
```csharp
// Luôn return ApiResult cho tất cả endpoints
public interface IProductService
{
    Task<ApiResult<List<Product>>> GetAllAsync();
    Task<ApiResult<Product>> GetByIdAsync(int id);
    Task<ApiResult<Product>> CreateAsync(CreateProductRequest request);
}
```

2. **Error Handling**
```csharp
try
{
    var result = await _service.ProcessAsync();
    return ApiResult.Success(result);
}
catch (NotFoundException ex)
{
    return ApiResult.Error<ProductDto>(ex.Message, 404);
}
catch (Exception ex)
{
    LoggerHelper.Error("Error processing request", ex);
    return ApiResult.Error<ProductDto>("An unexpected error occurred", 500);
}
```

3. **Extension Methods**
```csharp
public static class ApiResultExtensions
{
    public static ApiResult<T> ToApiResult<T>(this T data)
    {
        return ApiResult.Success(data);
    }
    
    public static PaginatedApiResult<T> ToPaginatedResult<T>(
        this T data,
        int page,
        int pageSize,
        int totalItems)
    {
        return new PaginatedApiResult<T>
        {
            Success = true,
            Data = data,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }
}
```

4. **Custom Middleware**
```csharp
public class ApiResultMiddleware
{
    private readonly RequestDelegate _next;
    
    public ApiResultMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var result = ApiResult.Error<object>(ex.Message, 500);
        context.Response.StatusCode = 500;
        return context.Response.WriteAsJsonAsync(result);
    }
}
``` 