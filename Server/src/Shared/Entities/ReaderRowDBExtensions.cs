using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public static class ReaderExtensions
    {
        /// <summary>
        /// Trả về chuỗi từ cột, hoặc giá trị mặc định nếu là NULL
        /// </summary>
        public static string GetSafeString(this DbDataReader reader, string column, string defaultValue = "")
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue : reader.GetString(index);
        }

        /// <summary>
        /// Trả về số nguyên (int) từ cột, hoặc giá trị mặc định nếu là NULL
        /// </summary>
        public static int GetSafeInt32(this DbDataReader reader, string column, int defaultValue = 0)
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue : reader.GetInt32(index);
        }

        /// <summary>
        /// Trả về giá trị boolean từ cột, hoặc giá trị mặc định nếu là NULL
        /// </summary>
        public static bool GetSafeBoolean(this DbDataReader reader, string column, bool defaultValue = false)
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue : reader.GetBoolean(index);
        }

        /// <summary>
        /// Trả về DateTime từ cột, hoặc giá trị mặc định nếu là NULL
        /// </summary>
        public static DateTime GetSafeDateTime(this DbDataReader reader, string column, DateTime? defaultValue = null)
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue ?? DateTime.MinValue : reader.GetDateTime(index);
        }

        /// <summary>
        /// Trả về giá trị decimal từ cột, hoặc giá trị mặc định nếu là NULL
        /// (phù hợp với kiểu dữ liệu money trong SQL Server)
        /// </summary>
        public static decimal GetSafeDecimal(this DbDataReader reader, string column, decimal defaultValue = 0m)
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue : reader.GetDecimal(index);
        }

        /// <summary>
        /// Trả về double từ cột, hoặc giá trị mặc định nếu là NULL
        /// </summary>
        public static double GetSafeDouble(this DbDataReader reader, string column, double defaultValue = 0.0)
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue : reader.GetDouble(index);
        }

        /// <summary>
        /// Trả về Guid từ cột, hoặc Guid.Empty nếu là NULL
        /// </summary>
        public static Guid GetSafeGuid(this DbDataReader reader, string column, Guid? defaultValue = null)
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue ?? Guid.Empty : reader.GetGuid(index);
        }

        /// <summary>
        /// Trả về giá trị long (Int64) từ cột, hoặc giá trị mặc định nếu là NULL
        /// </summary>
        public static long GetSafeInt64(this DbDataReader reader, string column, long defaultValue = 0L)
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue : reader.GetInt64(index);
        }

        /// <summary>
        /// Trả về giá trị float từ cột, hoặc giá trị mặc định nếu là NULL
        /// </summary>
        public static float GetSafeFloat(this DbDataReader reader, string column, float defaultValue = 0f)
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue : reader.GetFloat(index);
        }

        /// <summary>
        /// Trả về giá trị short (Int16) từ cột, hoặc giá trị mặc định nếu là NULL
        /// </summary>
        public static short GetSafeInt16(this DbDataReader reader, string column, short defaultValue = 0)
        {
            var index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue : reader.GetInt16(index);
        }
    }
}
