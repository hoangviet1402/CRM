# HttpClientHelper

Helper class để gọi HTTP requests một cách dễ dàng với nhiều tính năng hữu ích.

## Overview

`HttpClientHelper` là một static class cung cấp các phương thức để thực hiện HTTP requests một cách dễ dàng và an toàn. Helper này bao gồm các tính năng:

- Basic HTTP methods (GET, POST, PUT, DELETE)
- Custom headers và authentication
- File upload (single/multiple)
- Retry logic
- Parallel requests
- Query parameters
- Form data submission
- Error handling
- Logging integration

## Installation

Project đã có sẵn các package cần thiết:
```xml
<ItemGroup>
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="System.Text.Json" Version="6.0.0" />
</ItemGroup>
```

## Configuration

### Basic Configuration
```csharp
// Cấu hình timeout
HttpClientHelper.SetTimeout(TimeSpan.FromSeconds(30));

// Thêm header mặc định
HttpClientHelper.AddDefaultHeader("User-Agent", "MyApp/1.0");

// Cấu hình base URL
HttpClientHelper.SetBaseUrl("https://api.example.com");
```

## API Reference

### Basic HTTP Methods

#### GET
```csharp
public static async Task<T?> GetAsync<T>(
    string url, 
    Dictionary<string, string>? headers = null, 
    string? bearerToken = null)
```

#### POST
```csharp
public static async Task<TResponse?> PostAsync<TRequest, TResponse>(
    string url,
    TRequest data,
    Dictionary<string, string>? headers = null,
    string? bearerToken = null)
```

#### PUT
```csharp
public static async Task<TResponse?> PutAsync<TRequest, TResponse>(
    string url,
    TRequest data,
    Dictionary<string, string>? headers = null,
    string? bearerToken = null)
```

#### DELETE
```csharp
public static async Task<bool> DeleteAsync(
    string url,
    Dictionary<string, string>? headers = null,
    string? bearerToken = null)
```

### Advanced Methods

#### GET with Query Parameters
```csharp
public static async Task<T?> GetWithQueryAsync<T>(
    string baseUrl,
    Dictionary<string, string> queryParams,
    Dictionary<string, string>? headers = null,
    string? bearerToken = null)
```

#### POST Form Data
```csharp
public static async Task<T?> PostFormAsync<T>(
    string url,
    Dictionary<string, string> formData,
    Dictionary<string, string>? headers = null,
    string? bearerToken = null)
```

#### Upload Files
```csharp
public static async Task<T?> PostMultipartAsync<T>(
    string url,
    Dictionary<string, string> formData,
    Dictionary<string, (string fileName, byte[] fileContent, string contentType)> files,
    Dictionary<string, string>? headers = null,
    string? bearerToken = null)
```

## Usage Examples

### Basic GET Request
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

### File Upload Example
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

### Retry Logic Example
```csharp
var data = await HttpClientHelper.SendWithRetryAsync(
    async () => await HttpClientHelper.GetAsync<ResponseType>("https://api.example.com/data"),
    maxRetries: 3,
    delayMilliseconds: 1000
);
```

## Error Handling

```csharp
try
{
    var data = await HttpClientHelper.GetAsync<ResponseType>("https://api.example.com/data");
    // Process data
}
catch (HttpRequestException ex)
{
    // Handle network or HTTP-specific errors
    LoggerHelper.Error($"HTTP request failed: {ex.Message}", ex);
}
catch (Exception ex)
{
    // Handle other errors
    LoggerHelper.Error($"An error occurred: {ex.Message}", ex);
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