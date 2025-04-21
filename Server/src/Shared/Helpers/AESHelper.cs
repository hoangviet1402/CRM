using System.Security.Cryptography;
using System.Text;

namespace Shared.Helpers;

public static class AESHelper
{
    // Key size: 256 bits = 32 bytes
    private static readonly string DefaultKey = "YourSecretKey123!@#$%^&*()+=-0987";
    
    public static string Encrypt(string plainText, string? key = null)
    {
        try
        {
            string useKey = key ?? DefaultKey;
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(useKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public static string Decrypt(string cipherText, string? key = null)
    {
        try
        {
            string useKey = key ?? DefaultKey;
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(useKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 