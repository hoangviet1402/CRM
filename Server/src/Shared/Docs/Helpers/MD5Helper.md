# MD5Helper

Helper class để tạo MD5 hash từ strings, files và byte arrays.

## Overview

`MD5Helper` là một static class cung cấp các phương thức để tạo MD5 hash. Helper này hỗ trợ:

- Hash strings với encoding tùy chọn
- Hash files với buffered reading
- Hash byte arrays
- Kiểm tra hash matches
- Hỗ trợ nhiều output formats (hex, base64)

**Lưu ý**: MD5 không được khuyến nghị sử dụng cho mật khẩu hoặc bảo mật. Nó chỉ nên được dùng cho checksum hoặc file verification.

## Installation

Project đã có sẵn các dependencies cần thiết trong .NET Core.

## API Reference

### String Hashing

```csharp
// Hash string với default encoding (UTF8)
public static string GetMD5Hash(string input)

// Hash string với custom encoding
public static string GetMD5Hash(string input, Encoding encoding)
```

### File Hashing

```csharp
// Hash file từ path
public static string GetMD5HashFromFile(string filePath)

// Hash file từ stream
public static string GetMD5HashFromStream(Stream stream)
```

### Byte Array Hashing

```csharp
// Hash byte array
public static string GetMD5Hash(byte[] input)

// Get raw hash bytes
public static byte[] GetMD5Bytes(byte[] input)
```

### Hash Verification

```csharp
// Verify string hash
public static bool VerifyMD5Hash(string input, string hash)

// Verify file hash
public static bool VerifyFileMD5Hash(string filePath, string hash)
```

## Usage Examples

### Hash Strings
```csharp
// Hash string đơn giản
string hash = MD5Helper.GetMD5Hash("Hello World");
// Output: "b10a8db164e0754105b7a99be72e3fe5"

// Hash với custom encoding
string hashGB = MD5Helper.GetMD5Hash("你好", Encoding.GetEncoding("GB2312"));
```

### Hash Files
```csharp
// Hash file
string fileHash = MD5Helper.GetMD5HashFromFile("path/to/file.pdf");

// Hash với progress reporting
using (var stream = File.OpenRead("large_file.zip"))
{
    string hash = MD5Helper.GetMD5HashFromStream(stream, progress =>
    {
        Console.WriteLine($"Progress: {progress}%");
    });
}
```

### Verify Hashes
```csharp
// Verify string hash
bool isMatch = MD5Helper.VerifyMD5Hash(
    "Hello World",
    "b10a8db164e0754105b7a99be72e3fe5"
);

// Verify file hash
bool isFileMatch = MD5Helper.VerifyFileMD5Hash(
    "path/to/file.pdf",
    "expected-hash-here"
);
```

### Batch Processing
```csharp
// Hash nhiều files
var files = Directory.GetFiles("path/to/dir", "*.pdf");
var hashes = new Dictionary<string, string>();

foreach (var file in files)
{
    hashes[file] = MD5Helper.GetMD5HashFromFile(file);
}
```

## Best Practices

1. **Buffer Size cho Files Lớn**
```csharp
// Sử dụng buffer size phù hợp cho files lớn
const int BufferSize = 1024 * 1024; // 1MB buffer
using (var stream = File.OpenRead("large_file.zip"))
{
    string hash = MD5Helper.GetMD5HashFromStream(stream, bufferSize: BufferSize);
}
```

2. **Error Handling**
```csharp
try
{
    string hash = MD5Helper.GetMD5HashFromFile("file.pdf");
}
catch (FileNotFoundException ex)
{
    LoggerHelper.Error("File not found", ex);
}
catch (IOException ex)
{
    LoggerHelper.Error("Error reading file", ex);
}
```

3. **Progress Reporting cho Files Lớn**
```csharp
void ProcessLargeFile(string filePath)
{
    try
    {
        using var stream = File.OpenRead(filePath);
        string hash = MD5Helper.GetMD5HashFromStream(stream, progress =>
        {
            LoggerHelper.Information("Hash progress: {Progress}%", progress);
        });
    }
    catch (Exception ex)
    {
        LoggerHelper.Error("Failed to hash file", ex);
    }
}
```

4. **Case Sensitivity**
```csharp
// So sánh hashes nên ignore case
bool CompareHashes(string hash1, string hash2)
{
    return string.Equals(
        hash1, 
        hash2, 
        StringComparison.OrdinalIgnoreCase
    );
}
```

## Security Considerations

1. **KHÔNG sử dụng cho Passwords**
```csharp
// KHÔNG LÀM THẾ NÀY
string passwordHash = MD5Helper.GetMD5Hash(password); // Không an toàn!

// Thay vào đó sử dụng
string passwordHash = BCrypt.HashPassword(password);
```

2. **Sử dụng cho File Verification**
```csharp
// Tốt - Sử dụng để verify file integrity
string downloadedFileHash = MD5Helper.GetMD5HashFromFile("downloaded.zip");
bool isValid = downloadedFileHash == expectedHash;
```

3. **Sử dụng cho Caching**
```csharp
// Tốt - Sử dụng làm cache key
string cacheKey = MD5Helper.GetMD5Hash(complexObjectJson);
cache.Set(cacheKey, data);
``` 