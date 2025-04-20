# HttpClientHelper Documentation

Helper class để gọi HTTP requests một cách dễ dàng với nhiều tính năng hữu ích.

## Mục lục
- [Cài đặt](#cài-đặt)
- [Cấu hình](#cấu-hình)
- [Basic Requests](#basic-requests)
- [Advanced Requests](#advanced-requests)
- [Upload Files](#upload-files)
- [Retry và Parallel](#retry-và-parallel)

## Cài đặt

Project đã có sẵn các package cần thiết:
```xml
<ItemGroup>
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="System.Text.Json" Version="6.0.0" />
</ItemGroup>
```

## Cấu hình

### Cấu hình cơ bản
```csharp
// Cấu hình timeout
HttpClientHelper.SetTimeout(TimeSpan.FromSeconds(30));

// Thêm header mặc định
HttpClientHelper.AddDefaultHeader("User-Agent", "MyApp/1.0");

// Cấu hình base URL
HttpClientHelper.SetBaseUrl("https://api.example.com");
```

## Basic Requests

### GET Request
```csharp
// GET đơn giản
var data = await HttpClientHelper.GetAsync<ResponseType>("https://api.example.com/data");

// GET với bearer token
var data = await HttpClientHelper.GetAsync<ResponseType>(
    "https://api.example.com/data",
    bearerToken: "your-token"
);

// GET với custom headers
var headers = new Dictionary<string, string>
{
    ["X-Custom-Header"] = "Value",
    ["Accept-Language"] = "vi-VN"
};

var data = await HttpClientHelper.GetAsync<ResponseType>(
    "https://api.example.com/data",
    headers: headers
);
```

### POST Request
```csharp
// POST với body
var response = await HttpClientHelper.PostAsync<RequestType, ResponseType>(
    "https://api.example.com/create",
    new RequestType { 
        Name = "Test",
        Value = 123
    }
);

// POST với headers và token
var headers = new Dictionary<string, string>
{
    ["X-API-Key"] = "your-api-key"
};

var response = await HttpClientHelper.PostAsync<RequestType, ResponseType>(
    "https://api.example.com/create",
    data: new RequestType { /* data */ },
    headers: headers,
    bearerToken: "your-token"
);
```

### PUT Request
```csharp
var response = await HttpClientHelper.PutAsync<RequestType, ResponseType>(
    "https://api.example.com/update/123",
    new RequestType { 
        Name = "Updated",
        Value = 456
    }
);
```

### DELETE Request
```csharp
var success = await HttpClientHelper.DeleteAsync("https://api.example.com/delete/123");
```

## Advanced Requests

### GET với Query Parameters
```csharp
var queryParams = new Dictionary<string, string>
{
    ["search"] = "keyword",
    ["page"] = "1",
    ["pageSize"] = "10"
};

var data = await HttpClientHelper.GetWithQueryAsync<ResponseType>(
    "https://api.example.com/search",
    queryParams
);
```

### POST Form Data
```csharp
var formData = new Dictionary<string, string>
{
    ["username"] = "john",
    ["email"] = "john@example.com"
};

var response = await HttpClientHelper.PostFormAsync<ResponseType>(
    "https://api.example.com/submit",
    formData
);
```

## Upload Files

### Upload Single File
```csharp
var formData = new Dictionary<string, string>
{
    ["description"] = "My document"
};

var files = new Dictionary<string, (string, byte[], string)>
{
    ["file"] = ("document.pdf", File.ReadAllBytes("document.pdf"), "application/pdf")
};

var result = await HttpClientHelper.PostMultipartAsync<ResponseType>(
    "https://api.example.com/upload",
    formData,
    files
);
```

### Upload Multiple Files
```csharp
var formData = new Dictionary<string, string>
{
    ["description"] = "Multiple files"
};

var files = new Dictionary<string, (string, byte[], string)>
{
    ["file1"] = ("doc1.pdf", File.ReadAllBytes("doc1.pdf"), "application/pdf"),
    ["file2"] = ("image.jpg", File.ReadAllBytes("image.jpg"), "image/jpeg")
};

var result = await HttpClientHelper.PostMultipartAsync<ResponseType>(
    "https://api.example.com/upload-multiple",
    formData,
    files
);
```

## Retry và Parallel

### Retry khi thất bại
```csharp
var data = await HttpClientHelper.SendWithRetryAsync(
    async () => await HttpClientHelper.GetAsync<ResponseType>("https://api.example.com/data"),
    maxRetries: 3,
    delayMilliseconds: 1000
);
```

### Gọi nhiều requests song song
```csharp
var requests = new Dictionary<string, Func<Task<ResponseType?>>>
{
    ["request1"] = () => HttpClientHelper.GetAsync<ResponseType>("https://api1.example.com"),
    ["request2"] = () => HttpClientHelper.GetAsync<ResponseType>("https://api2.example.com")
};

var results = await HttpClientHelper.SendParallelAsync(
    requests,
    maxParallelism: 3
);

// Kết quả
var result1 = results["request1"];
var result2 = results["request2"];
```

## Error Handling

Tất cả các methods đều tự động log lỗi qua LoggerHelper và throw exception khi có lỗi. Bạn nên handle exception như sau:

```csharp
try
{
    var data = await HttpClientHelper.GetAsync<ResponseType>("https://api.example.com/data");
    // Process data
}
catch (HttpRequestException ex)
{
    // Handle network or HTTP-specific errors
    Console.WriteLine($"HTTP request failed: {ex.Message}");
}
catch (Exception ex)
{
    // Handle other errors
    Console.WriteLine($"An error occurred: {ex.Message}");
}
```

## Best Practices

1. **Sử dụng Interface/Class cho Request/Response**
```csharp
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

// Sử dụng
var response = await HttpClientHelper.PostAsync<LoginRequest, LoginResponse>(
    "https://api.example.com/login",
    new LoginRequest 
    { 
        Username = "john",
        Password = "password123"
    }
);
```

2. **Tạo Service Class để đóng gói logic gọi API**
```csharp
public class UserService
{
    private const string BaseUrl = "https://api.example.com";
    private readonly string _token;

    public UserService(string token)
    {
        _token = token;
    }

    public async Task<UserResponse> GetUserAsync(int userId)
    {
        return await HttpClientHelper.GetAsync<UserResponse>(
            $"{BaseUrl}/users/{userId}",
            bearerToken: _token
        );
    }
}
```

3. **Sử dụng Constants cho URLs và Headers**
```csharp
public static class ApiConstants
{
    public const string BaseUrl = "https://api.example.com";
    public const string ApiKeyHeader = "X-API-Key";
    public const string LanguageHeader = "Accept-Language";
}
``` 