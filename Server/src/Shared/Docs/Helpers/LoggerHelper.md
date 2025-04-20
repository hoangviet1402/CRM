# LoggerHelper

Helper class để log thông tin, warnings và errors với nhiều levels khác nhau.

## Overview

`LoggerHelper` là một static class cung cấp các phương thức để log thông tin một cách dễ dàng và nhất quán. Helper này sử dụng Serilog làm logging framework và hỗ trợ:

- Multiple log levels (Debug, Info, Warning, Error)
- Exception logging với stack trace
- Structured logging
- File và Console output
- Log rotation
- Custom log formatting

## Installation

Project đã có sẵn Serilog package:
```xml
<ItemGroup>
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.0" />
</ItemGroup>
```

## Configuration

### Basic Configuration
```csharp
// Cấu hình trong Program.cs hoặc Startup.cs
LoggerHelper.Configure(options =>
{
    options.LogFilePath = "logs/app.log";
    options.MinimumLevel = LogLevel.Information;
    options.RetainedFileCountLimit = 31; // Giữ logs 31 ngày
    options.FileSizeLimitBytes = 10 * 1024 * 1024; // 10MB mỗi file
});
```

## API Reference

### Log Levels

#### Debug
```csharp
public static void Debug(string message, params object[] args)
```

#### Information
```csharp
public static void Information(string message, params object[] args)
```

#### Warning
```csharp
public static void Warning(string message, params object[] args)
public static void Warning(string message, Exception ex)
```

#### Error
```csharp
public static void Error(string message, params object[] args)
public static void Error(string message, Exception ex)
```

### Structured Logging
```csharp
public static void Information<T>(string message, T context)
```

## Usage Examples

### Basic Logging
```csharp
// Log thông tin
LoggerHelper.Information("User {Username} logged in successfully", username);

// Log warning
LoggerHelper.Warning("Database connection pool is running low: {AvailableConnections}", connections);

// Log error với exception
try
{
    // Some code that might throw
}
catch (Exception ex)
{
    LoggerHelper.Error("Failed to process payment", ex);
}
```

### Structured Logging
```csharp
var order = new Order
{
    Id = "123",
    Amount = 1500000,
    Status = "Pending"
};

LoggerHelper.Information("Processing order: {@Order}", order);
```

### Context Logging
```csharp
var context = new
{
    UserId = "123",
    Action = "checkout",
    CartValue = 1500000
};

LoggerHelper.Information("User initiated checkout: {Context}", context);
```

### Performance Logging
```csharp
using (LoggerHelper.BeginScope("Processing large file"))
{
    LoggerHelper.Information("Started processing");
    // Do work
    LoggerHelper.Information("Completed processing");
}
```

## Error Handling

```csharp
try
{
    // Some risky operation
    throw new Exception("Something went wrong");
}
catch (Exception ex)
{
    // Log full exception details
    LoggerHelper.Error("Operation failed", ex);
    
    // Log với custom context
    LoggerHelper.Error("Operation failed: {ErrorCode} - {ErrorMessage}", 
        ex.HResult, 
        ex.Message);
}
```

## Best Practices

1. **Sử dụng Message Templates**
```csharp
// Tốt
LoggerHelper.Information("User {UserId} performed {Action}", userId, action);

// Không tốt
LoggerHelper.Information($"User {userId} performed {action}");
```

2. **Log Levels Phù Hợp**
```csharp
// Debug - Chi tiết để troubleshoot
LoggerHelper.Debug("Connection pool status: {Status}", status);

// Information - Thông tin quan trọng về flow
LoggerHelper.Information("Order {OrderId} created successfully", orderId);

// Warning - Vấn đề tiềm ẩn nhưng chưa critical
LoggerHelper.Warning("High memory usage detected: {Usage}%", memoryUsage);

// Error - Lỗi cần xử lý ngay
LoggerHelper.Error("Payment processing failed", exception);
```

3. **Structured Data**
```csharp
// Log object dưới dạng structured
var user = new { Id = "123", Name = "John" };
LoggerHelper.Information("User details: {@User}", user);

// Log dictionary
var metrics = new Dictionary<string, int> 
{
    ["RequestCount"] = 100,
    ["ErrorCount"] = 5
};
LoggerHelper.Information("Daily metrics: {@Metrics}", metrics);
```

4. **Exception Handling**
```csharp
try
{
    // Some code
}
catch (Exception ex) when (ex is not OperationCanceledException)
{
    // Log chi tiết exception và context
    LoggerHelper.Error(
        ex,
        "Operation failed: {Operation} - {Context}",
        "ProcessOrder",
        new { OrderId = "123", Stage = "Payment" }
    );
}
``` 