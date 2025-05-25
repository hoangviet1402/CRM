using System;
using System.Data;
using System.Collections.Generic;

namespace Shared.Helpers
{
    public static class DataRowExtensions
    {
        public static int GetSafeInt32(this DataRow row, string column, int defaultValue = 0)
        {
            if (row == null)
                return defaultValue;

            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return defaultValue;

            var value = row[column];
            if (value is int intValue)
                return intValue;
            if (value is decimal decimalValue)
                return (int)decimalValue;
            if (value is double doubleValue)
                return (int)doubleValue;
            if (value is string stringValue && int.TryParse(stringValue, out int parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string GetSafeString(this DataRow row, string column, string defaultValue = "")
        {
            if (row == null)
                return defaultValue;

            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return defaultValue;

            return row[column].ToString() ?? defaultValue;
        }

        public static bool GetSafeBoolean(this DataRow row, string column, bool defaultValue = false)
        {
            if (row == null)
                return defaultValue;

            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return defaultValue;

            var value = row[column];
            if (value is bool boolValue)
                return boolValue;
            if (value is int intValue)
                return intValue != 0;
            if (value is string stringValue && bool.TryParse(stringValue, out bool parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime GetSafeDateTime(this DataRow row, string column, DateTime? defaultValue = null)
        {
            if (row == null)
                return defaultValue ?? DateTime.MinValue;

            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return defaultValue ?? DateTime.MinValue;

            var value = row[column];
            if (value is DateTime dateTimeValue)
                return dateTimeValue;
            if (value is string stringValue && DateTime.TryParse(stringValue, out DateTime parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {
                return defaultValue ?? DateTime.MinValue;
            }
        }

        public static decimal GetSafeDecimal(this DataRow row, string column, decimal defaultValue = 0)
        {
            if (row == null)
                return defaultValue;

            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return defaultValue;

            var value = row[column];
            if (value is decimal decimalValue)
                return decimalValue;
            if (value is int intValue)
                return intValue;
            if (value is double doubleValue)
                return (decimal)doubleValue;
            if (value is string stringValue && decimal.TryParse(stringValue, out decimal parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float GetSafeFloat(this DataRow row, string column, float defaultValue = 0)
        {
            if (row == null)
                return defaultValue;

            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return defaultValue;

            var value = row[column];
            if (value is float floatValue)
                return floatValue;
            if (value is int intValue)
                return intValue;
            if (value is double doubleValue)
                return (float)doubleValue;
            if (value is string stringValue && float.TryParse(stringValue, out float parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToSingle(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static long GetSafeInt64(this DataRow row, string column, long defaultValue = 0)
        {
            if (row == null)
                return defaultValue;

            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return defaultValue;

            var value = row[column];
            if (value is long longValue)
                return longValue;
            if (value is int intValue)
                return intValue;
            if (value is decimal decimalValue)
                return (long)decimalValue;
            if (value is string stringValue && long.TryParse(stringValue, out long parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        // Dictionary extensions
        public static int GetSafeInt32(this Dictionary<string, object> dict, string key, int defaultValue = 0)
        {
            if (dict == null || !dict.ContainsKey(key) || dict[key] == null)
                return defaultValue;

            var value = dict[key];
            if (value is int intValue)
                return intValue;
            if (value is decimal decimalValue)
                return (int)decimalValue;
            if (value is double doubleValue)
                return (int)doubleValue;
            if (value is string stringValue && int.TryParse(stringValue, out int parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string GetSafeString(this Dictionary<string, object> dict, string key, string defaultValue = "")
        {
            if (dict == null || !dict.ContainsKey(key) || dict[key] == null)
                return defaultValue;

            return dict[key].ToString() ?? defaultValue;
        }

        public static bool GetSafeBoolean(this Dictionary<string, object> dict, string key, bool defaultValue = false)
        {
            if (dict == null || !dict.ContainsKey(key) || dict[key] == null)
                return defaultValue;

            var value = dict[key];
            if (value is bool boolValue)
                return boolValue;
            if (value is int intValue)
                return intValue != 0;
            if (value is string stringValue && bool.TryParse(stringValue, out bool parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime GetSafeDateTime(this Dictionary<string, object> dict, string key, DateTime? defaultValue = null)
        {
            if (dict == null || !dict.ContainsKey(key) || dict[key] == null)
                return defaultValue ?? DateTime.MinValue;

            var value = dict[key];
            if (value is DateTime dateTimeValue)
                return dateTimeValue;
            if (value is string stringValue && DateTime.TryParse(stringValue, out DateTime parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {
                return defaultValue ?? DateTime.MinValue;
            }
        }

        public static decimal GetSafeDecimal(this Dictionary<string, object> dict, string key, decimal defaultValue = 0)
        {
            if (dict == null || !dict.ContainsKey(key) || dict[key] == null)
                return defaultValue;

            var value = dict[key];
            if (value is decimal decimalValue)
                return decimalValue;
            if (value is int intValue)
                return intValue;
            if (value is double doubleValue)
                return (decimal)doubleValue;
            if (value is string stringValue && decimal.TryParse(stringValue, out decimal parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float GetSafeFloat(this Dictionary<string, object> dict, string key, float defaultValue = 0)
        {
            if (dict == null || !dict.ContainsKey(key) || dict[key] == null)
                return defaultValue;

            var value = dict[key];
            if (value is float floatValue)
                return floatValue;
            if (value is int intValue)
                return intValue;
            if (value is double doubleValue)
                return (float)doubleValue;
            if (value is string stringValue && float.TryParse(stringValue, out float parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToSingle(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static long GetSafeInt64(this Dictionary<string, object> dict, string key, long defaultValue = 0)
        {
            if (dict == null || !dict.ContainsKey(key) || dict[key] == null)
                return defaultValue;

            var value = dict[key];
            if (value is long longValue)
                return longValue;
            if (value is int intValue)
                return intValue;
            if (value is decimal decimalValue)
                return (long)decimalValue;
            if (value is string stringValue && long.TryParse(stringValue, out long parsedValue))
                return parsedValue;

            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
} 