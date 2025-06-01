using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Shared.Extensions;
using Shared.Helpers;
using Infrastructure.DbContext;
using System.Linq;
using System.Collections.Concurrent;
using System.Reflection;

namespace Infrastructure.StoredProcedureMapperModule;

/// <summary>
/// Lớp xử lý việc thực thi stored procedure và quản lý kết nối database
/// </summary>
public class StoredProcedureMapperModule
{
    /// <summary>
    /// Quản lý kết nối database
    /// </summary>
    protected readonly DatabaseConnection _dbConnection;

    /// <summary>
    /// Tên kết nối database (ví dụ: "TanCa", "TanTam", "TanLam")
    /// </summary>
    protected readonly string _connectionName;

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();
    private const int DEFAULT_COMMAND_TIMEOUT = 30; // 30 giây

    /// <summary>
    /// Khởi tạo StoredProcedureMapperModule
    /// </summary>
    /// <param name="dbConnection">Quản lý kết nối database</param>
    /// <param name="connectionName">Tên kết nối database</param>
    public StoredProcedureMapperModule(DatabaseConnection dbConnection , string connectionName = "TanCa")
    {
        _dbConnection = dbConnection;
        _connectionName = connectionName;
    }

    /// <summary>
    /// Tạo kết nối SQL mới
    /// </summary>
    /// <returns>SqlConnection mới</returns>
    /// <exception cref="InvalidOperationException">Khi dbConnection hoặc connectionName không được cung cấp</exception>
    protected SqlConnection CreateConnection()
    {
        if (_dbConnection == null || string.IsNullOrEmpty(_connectionName))
        {
            throw new InvalidOperationException("DatabaseConnection and connectionName must be provided");
        }
        return _dbConnection.CreateConnection(_connectionName);
    }

    private PropertyInfo[] GetCachedProperties(Type type)
    {
        return _propertyCache.GetOrAdd(type, t => t.GetProperties());
    }

    private T MapDataRowToEntity<T>(DataRow row, DataColumnCollection columns) where T : new()
    {
        var entity = new T();
        var properties = GetCachedProperties(typeof(T));
        var columnNames = new HashSet<string>(columns.Cast<DataColumn>().Select(c => c.ColumnName));

        foreach (var property in properties)
        {
            if (columnNames.Contains(property.Name))
            {
                var value = row[property.Name];
                if (value == DBNull.Value)
                {
                    // Nếu là nullable type, set giá trị null
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        property.SetValue(entity, null);
                    }
                    continue;
                }

                try
                {
                    // Xử lý Nullable Types
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                        if (underlyingType == typeof(int))
                            property.SetValue(entity, Convert.ToInt32(value));
                        else if (underlyingType == typeof(long))
                            property.SetValue(entity, Convert.ToInt64(value));
                        else if (underlyingType == typeof(decimal))
                            property.SetValue(entity, Convert.ToDecimal(value));
                        else if (underlyingType == typeof(double))
                            property.SetValue(entity, Convert.ToDouble(value));
                        else if (underlyingType == typeof(float))
                            property.SetValue(entity, Convert.ToSingle(value));
                        else if (underlyingType == typeof(bool))
                            property.SetValue(entity, Convert.ToBoolean(value));
                        else if (underlyingType == typeof(DateTime))
                            property.SetValue(entity, Convert.ToDateTime(value));
                        else
                            property.SetValue(entity, Convert.ChangeType(value, underlyingType));
                    }
                    // Xử lý Non-Nullable Types
                    else
                    {
                        if (property.PropertyType == typeof(int))
                            property.SetValue(entity, Convert.ToInt32(value));
                        else if (property.PropertyType == typeof(long))
                            property.SetValue(entity, Convert.ToInt64(value));
                        else if (property.PropertyType == typeof(decimal))
                            property.SetValue(entity, Convert.ToDecimal(value));
                        else if (property.PropertyType == typeof(double))
                            property.SetValue(entity, Convert.ToDouble(value));
                        else if (property.PropertyType == typeof(float))
                            property.SetValue(entity, Convert.ToSingle(value));
                        else if (property.PropertyType == typeof(bool))
                            property.SetValue(entity, Convert.ToBoolean(value));
                        else if (property.PropertyType == typeof(DateTime))
                            property.SetValue(entity, Convert.ToDateTime(value));
                        else if (property.PropertyType == typeof(string))
                            property.SetValue(entity, value.ToString());
                        else
                            property.SetValue(entity, Convert.ChangeType(value, property.PropertyType));
                    }
                }
                catch
                {
                    // Bỏ qua nếu không thể chuyển đổi kiểu dữ liệu
                }
            }
        }

        return entity;
    }

    /// <summary>
    /// Thực thi stored procedure với kiểu trả về tùy chọn
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về (bool, DataTable, int)</typeparam>
    /// <param name="storedProcedureName">Tên stored procedure</param>
    /// <param name="parameters">Các tham số đầu vào</param>
    /// <param name="outputParameters">Các tham số đầu ra (chỉ dùng khi T là bool hoặc int)</param>
    /// <param name="commandTimeout">Thời gian chờ đợi cho việc thực thi (giây)</param>
    /// <returns>
    /// - bool: True nếu thực thi thành công, False nếu thất bại
    /// - DataTable: Kết quả dạng bảng hoặc null nếu thất bại
    /// - int: Số lượng bản ghi bị ảnh hưởng hoặc 0 nếu thất bại
    /// </returns>
    public async Task<T?> ExecuteStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object>? parameters = null, Dictionary<string, object>? outputParameters = null, int? commandTimeout = null)
    {
        using var connection = CreateConnection();
        try
        {
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;
            command.CommandTimeout = commandTimeout ?? DEFAULT_COMMAND_TIMEOUT;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Value = param.Value ?? DBNull.Value;
                    
                    // Thêm kích thước cho tham số string
                    if (param.Value is string stringValue)
                    {
                        parameter.Size = stringValue.Length;
                    }
                    else if (param.Value == null)
                    {
                        parameter.Size = 100; // Kích thước mặc định cho null
                    }
                    
                    command.Parameters.Add(parameter);
                }
            }

            if (outputParameters != null)
            {
                foreach (var param in outputParameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Direction = ParameterDirection.Output;
                    parameter.Size = 100; // Kích thước mặc định cho output
                    command.Parameters.Add(parameter);
                }
            }

            if (typeof(T) == typeof(bool))
            {
                await command.ExecuteNonQueryAsync();
                
                if (outputParameters != null)
                {
                    foreach (IDataParameter parameter in command.Parameters)
                    {
                        if (parameter.Direction == ParameterDirection.Output)
                        {
                            outputParameters[parameter.ParameterName] = parameter.Value;
                        }
                    }
                }
                
                return (T)(object)true;
            }
            else if (typeof(T) == typeof(DataTable))
            {
                var dataTable = new DataTable();
                using var reader = await command.ExecuteReaderAsync();
                dataTable.Load(reader);
                return (T)(object)dataTable;
            }
            else if (typeof(T) == typeof(int))
            {
                await command.ExecuteNonQueryAsync();
                
                if (outputParameters != null)
                {
                    foreach (IDataParameter parameter in command.Parameters)
                    {
                        if (parameter.Direction == ParameterDirection.Output)
                        {
                            outputParameters[parameter.ParameterName] = parameter.Value;
                        }
                    }
                }
                
                // Trả về giá trị đầu tiên trong outputParameters nếu có
                if (outputParameters != null && outputParameters.Count > 0)
                {
                    var firstOutput = outputParameters.First();
                    if (firstOutput.Value is int intValue)
                        return (T)(object)intValue;
                    if (firstOutput.Value is decimal decimalValue)
                        return (T)(object)(int)decimalValue;
                    if (firstOutput.Value is double doubleValue)
                        return (T)(object)(int)doubleValue;
                    if (firstOutput.Value is string stringValue && int.TryParse(stringValue, out int parsedValue))
                        return (T)(object)parsedValue;
                }
                
                return (T)(object)0;
            }
            else
            {
                var dataTable = new DataTable();
                using var reader = await command.ExecuteReaderAsync();
                dataTable.Load(reader);

                if (dataTable.Rows.Count == 0)
                    return default;

                // Chỉ xử lý khi T là class và có constructor không tham số
                if (typeof(T).IsClass && !typeof(T).IsAbstract && typeof(T).GetConstructor(Type.EmptyTypes) != null)
                {
                    try
                    {
                        var entity = Activator.CreateInstance<T>();
                        var properties = GetCachedProperties(typeof(T));
                        var columnNames = new HashSet<string>(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));

                        foreach (var property in properties)
                        {
                            if (columnNames.Contains(property.Name))
                            {
                                var value = dataTable.Rows[0][property.Name];
                                if (value == DBNull.Value)
                                {
                                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        property.SetValue(entity, null);
                                    }
                                    continue;
                                }

                                try
                                {
                                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                                        property.SetValue(entity, Convert.ChangeType(value, underlyingType));
                                    }
                                    else
                                    {
                                        property.SetValue(entity, Convert.ChangeType(value, property.PropertyType));
                                    }
                                }
                                catch
                                {
                                    // Bỏ qua nếu không thể chuyển đổi kiểu dữ liệu
                                }
                            }
                        }
                        return entity;
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Error($"MapDataRowToEntity Error for type {typeof(T).Name}: {ex.Message}", ex);
                        return default;
                    }
                }
                
                return default;
            }
        }
        catch (SqlException ex) when (ex.Number == -2)
        {
            LoggerHelper.Error($"ExecuteStoredProcedureAsync Timeout Error: {ex.Message}", ex);
            throw new TimeoutException($"Stored procedure '{storedProcedureName}' execution timed out after {commandTimeout ?? DEFAULT_COMMAND_TIMEOUT} seconds", ex);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"ExecuteStoredProcedureAsync Error: {ex.Message}", ex);
            
            if (typeof(T) == typeof(bool))
                return (T)(object)false;
            //else if (typeof(T) == typeof(DataTable))
            //    return (T)(object)null;
            else if (typeof(T) == typeof(int))
                return (T)(object)0;
            else
                return default;
        }
    }

    public async Task<List<T>> ExecuteStoredProcedureListAsync<T>(string storedProcedureName, Dictionary<string, object>? parameters = null, Dictionary<string, object>? outputParameters = null, int? commandTimeout = null) where T : new()
    {
        using var connection = CreateConnection();
        try
        {
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;
            command.CommandTimeout = commandTimeout ?? DEFAULT_COMMAND_TIMEOUT;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }
            }

            if (outputParameters != null)
            {
                foreach (var param in outputParameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    parameter.Direction = ParameterDirection.Output;
                    parameter.Size = 100; // Kích thước mặc định cho output
                    command.Parameters.Add(parameter);
                }
            }

            var result = new List<T>();
            using var reader = await command.ExecuteReaderAsync();
            
            // Cache column indices
            var columnIndices = new Dictionary<string, int>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnIndices[reader.GetName(i)] = i;
            }

            // Cache property info
            var properties = GetCachedProperties(typeof(T));

            while (await reader.ReadAsync())
            {
                var entity = new T();
                foreach (var property in properties)
                {
                    if (columnIndices.TryGetValue(property.Name, out int columnIndex))
                    {
                        var value = reader.GetValue(columnIndex);
                        if (value != DBNull.Value)
                        {
                            try
                            {
                                // Xử lý Nullable Types
                                if (property.PropertyType.IsGenericType && 
                                    property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                                    property.SetValue(entity, Convert.ChangeType(value, underlyingType));
                                }
                                else
                                {
                                    property.SetValue(entity, Convert.ChangeType(value, property.PropertyType));
                                }
                            }
                            catch
                            {
                                // Bỏ qua nếu không thể chuyển đổi kiểu dữ liệu
                            }
                        }
                    }
                }
                result.Add(entity);
            }

            return result;
        }
        catch (SqlException ex) when (ex.Number == -2)
        {
            LoggerHelper.Error($"ExecuteStoredProcedureListAsync Timeout Error: {ex.Message}", ex);
            throw new TimeoutException($"Stored procedure '{storedProcedureName}' execution timed out after {commandTimeout ?? DEFAULT_COMMAND_TIMEOUT} seconds", ex);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"ExecuteStoredProcedureListAsync Error: {ex.Message}", ex);
            return new List<T>();
        }
    }
} 