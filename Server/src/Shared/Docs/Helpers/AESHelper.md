# AESHelper

Helper class để mã hóa và giải mã data sử dụng AES (Advanced Encryption Standard).

## Overview

`AESHelper` là một static class cung cấp các phương thức để mã hóa và giải mã data sử dụng AES. Helper này hỗ trợ:

- Mã hóa/giải mã strings
- Mã hóa/giải mã files
- Mã hóa/giải mã byte arrays
- Key generation và management
- IV (Initialization Vector) handling
- Multiple encoding options

## Installation

Project đã có sẵn các dependencies cần thiết trong .NET Core.

## API Reference

### String Encryption/Decryption

```csharp
// Mã hóa string
public static string Encrypt(string plainText, string key)

// Giải mã string
public static string Decrypt(string cipherText, string key)
```

### File Encryption/Decryption

```csharp
// Mã hóa file
public static void EncryptFile(string inputFile, string outputFile, string key)

// Giải mã file
public static void DecryptFile(string inputFile, string outputFile, string key)
```

### Byte Array Encryption/Decryption

```csharp
// Mã hóa bytes
public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)

// Giải mã bytes
public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
```

### Key Management

```csharp
// Generate key
public static string GenerateKey(int keySize = 256)

// Generate IV
public static byte[] GenerateIV()
```

## Usage Examples

### String Encryption
```csharp
// Mã hóa string
string key = AESHelper.GenerateKey();
string encrypted = AESHelper.Encrypt("Sensitive data", key);

// Giải mã string
string decrypted = AESHelper.Decrypt(encrypted, key);
```

### File Encryption
```csharp
// Mã hóa file
string key = AESHelper.GenerateKey();
AESHelper.EncryptFile(
    "sensitive.pdf",
    "sensitive.encrypted",
    key
);

// Giải mã file
AESHelper.DecryptFile(
    "sensitive.encrypted",
    "sensitive.decrypted.pdf",
    key
);
```

### Stream Encryption
```csharp
// Mã hóa stream
using (var inputStream = File.OpenRead("input.file"))
using (var outputStream = File.Create("output.encrypted"))
{
    AESHelper.EncryptStream(inputStream, outputStream, key);
}
```

### Secure Configuration
```csharp
// Mã hóa configuration values
public class SecureConfig
{
    private readonly string _key;
    
    public SecureConfig(string key)
    {
        _key = key;
    }
    
    public string EncryptValue(string value)
    {
        return AESHelper.Encrypt(value, _key);
    }
    
    public string DecryptValue(string encryptedValue)
    {
        return AESHelper.Decrypt(encryptedValue, _key);
    }
}
```

## Best Practices

1. **Key Management**
```csharp
// Generate và store key an toàn
public class KeyManager
{
    private const string KeyPath = "secure/keys/";
    
    public static string GetOrCreateKey(string keyName)
    {
        var path = Path.Combine(KeyPath, $"{keyName}.key");
        
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        
        var key = AESHelper.GenerateKey();
        Directory.CreateDirectory(KeyPath);
        File.WriteAllText(path, key);
        
        return key;
    }
}
```

2. **Error Handling**
```csharp
try
{
    string decrypted = AESHelper.Decrypt(encrypted, key);
}
catch (CryptographicException ex)
{
    LoggerHelper.Error("Decryption failed - invalid key or corrupted data", ex);
}
catch (Exception ex)
{
    LoggerHelper.Error("Unexpected error during decryption", ex);
}
```

3. **Large File Handling**
```csharp
public async Task EncryptLargeFileAsync(
    string inputFile,
    string outputFile,
    string key,
    IProgress<int> progress)
{
    try
    {
        await AESHelper.EncryptFileAsync(
            inputFile,
            outputFile,
            key,
            progress
        );
    }
    catch (Exception ex)
    {
        LoggerHelper.Error("File encryption failed", ex);
        throw;
    }
}
```

## Security Considerations

1. **Key Storage**
```csharp
// KHÔNG LÀM THẾ NÀY
public static string EncryptionKey = "hardcoded-key"; // Không an toàn!

// Thay vào đó sử dụng secure storage
public static string GetEncryptionKey()
{
    return Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
        ?? throw new InvalidOperationException("Encryption key not found");
}
```

2. **IV Management**
```csharp
// Sử dụng unique IV cho mỗi encryption
public static (string encrypted, string iv) EncryptWithIV(string data, string key)
{
    var iv = AESHelper.GenerateIV();
    var encrypted = AESHelper.Encrypt(data, key, iv);
    
    return (
        Convert.ToBase64String(encrypted),
        Convert.ToBase64String(iv)
    );
}
```

3. **Secure Data Handling**
```csharp
public class SecureDataHandler : IDisposable
{
    private readonly string _key;
    private bool _disposed;
    
    public SecureDataHandler(string key)
    {
        _key = key;
    }
    
    public string ProcessSensitiveData(string data)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SecureDataHandler));
            
        try
        {
            return AESHelper.Encrypt(data, _key);
        }
        finally
        {
            // Clear sensitive data from memory
            Array.Clear(Encoding.UTF8.GetBytes(data), 0, data.Length);
        }
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            // Clear key from memory
            Array.Clear(Encoding.UTF8.GetBytes(_key), 0, _key.Length);
            _disposed = true;
        }
    }
}
```

4. **Data Integrity**
```csharp
public class SecureMessage
{
    public string EncryptedData { get; set; }
    public string IV { get; set; }
    public string Signature { get; set; } // HMAC của encrypted data
    
    public static SecureMessage Create(string data, string key)
    {
        var iv = AESHelper.GenerateIV();
        var encrypted = AESHelper.Encrypt(data, key, iv);
        var signature = ComputeHMAC(encrypted, key);
        
        return new SecureMessage
        {
            EncryptedData = Convert.ToBase64String(encrypted),
            IV = Convert.ToBase64String(iv),
            Signature = signature
        };
    }
    
    private static string ComputeHMAC(byte[] data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(data);
        return Convert.ToBase64String(hash);
    }
} 