using System;
using System.Data;
using System.Collections.Generic;

namespace Shared.Helpers
{
    public static class DataRowExtensions
    {
        public static int GetSafeInt32(this DataRow row, string column, int defaultValue = 0)
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value
                ? Convert.ToInt32(row[column])
                : defaultValue;
        }

        public static string GetSafeString(this DataRow row, string column, string defaultValue = "")
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value
                ? row[column].ToString()
                : defaultValue;
        }

        public static bool GetSafeBoolean(this DataRow row, string column, bool defaultValue = false)
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value
                ? Convert.ToBoolean(row[column])
                : defaultValue;
        }

        public static DateTime GetSafeDateTime(this DataRow row, string column, DateTime? defaultValue = null)
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value
                ? Convert.ToDateTime(row[column])
                : (defaultValue ?? DateTime.MinValue);
        }

        public static int GetSafeInt32(this Dictionary<string, object> dict, string key, int defaultValue = 0)
        {
            return dict.ContainsKey(key) && dict[key] != null
                ? Convert.ToInt32(dict[key])
                : defaultValue;
        }

        public static string GetSafeString(this Dictionary<string, object> dict, string key, string defaultValue = "")
        {
            return dict.ContainsKey(key) && dict[key] != null
                ? dict[key].ToString()
                : defaultValue;
        }

        public static bool GetSafeBoolean(this Dictionary<string, object> dict, string key, bool defaultValue = false)
        {
            return dict.ContainsKey(key) && dict[key] != null
                ? Convert.ToBoolean(dict[key])
                : defaultValue;
        }

        public static DateTime GetSafeDateTime(this Dictionary<string, object> dict, string key, DateTime? defaultValue = null)
        {
            return dict.ContainsKey(key) && dict[key] != null
                ? Convert.ToDateTime(dict[key])
                : (defaultValue ?? DateTime.MinValue);
        }
    }
} 