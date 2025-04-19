using System.Security.Cryptography;
using System.Text;

namespace Shared.Helpers;

public static class MD5Helper
{
    public static string EncryptMD5(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        using (var md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2")); // Convert to hexadecimal
            }

            return sb.ToString();
        }
    }

    public static bool VerifyMD5(string input, string hash)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(hash))
        {
            return false;
        }

        string inputHash = EncryptMD5(input);
        return string.Equals(inputHash, hash, StringComparison.OrdinalIgnoreCase);
    }
} 