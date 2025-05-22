using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Utils
{
    public class Helper
    {
        /// <summary>
        /// Tạo chuỗi số ngẫu nhiên với độ dài được chỉ định
        /// </summary>
        /// <param name="length">Độ dài mong muốn của chuỗi số</param>
        /// <returns>Chuỗi số có độ dài được chỉ định</returns>
        public static string GenerateUniqueNumber(int length = 11)
        {
            // Lấy timestamp hiện tại (milliseconds)
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            // Tạo số ngẫu nhiên 3 chữ số
            Random random = new Random();
            int randomNum = random.Next(100, 999);
            
            // Kết hợp timestamp và số ngẫu nhiên
            string result = $"{timestamp}{randomNum}";
            
            // Đảm bảo độ dài theo yêu cầu bằng cách cắt hoặc thêm số 0
            if (result.Length > length)
            {
                result = result.Substring(0, length);
            }
            else if (result.Length < length)
            {
                result = result.PadLeft(length, '0');
            }
            
            return result;
        }

        public static string GenerateUniqueString(int length = 11)
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
