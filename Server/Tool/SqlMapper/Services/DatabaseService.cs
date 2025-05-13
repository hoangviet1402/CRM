using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SqlMapper.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger = null)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found");
            _logger = logger;
        }

        private SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            return connection;
        }

        public async Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string procedureName, object parameters = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            return await connection.QueryAsync<T>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<T> QueryStoredProcedureSingleAsync<T>(string procedureName, object parameters = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            return await connection.QuerySingleOrDefaultAsync<T>(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ExecuteStoredProcedureAsync(string procedureName, object parameters = null)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();
            return await connection.ExecuteAsync(
                procedureName,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<DataTable> GetStoredProcedureSchemaAsync(string procedureName, object parameters = null)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();
                
                _logger?.LogInformation($"Kết nối đến SQL Server thành công. Đang thực thi stored procedure: {procedureName}");
                
                // Create a command to execute the stored procedure
                using var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters if provided
                if (parameters != null)
                {
                    var properties = parameters.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(parameters);
                        command.Parameters.AddWithValue($"@{prop.Name}", value ?? DBNull.Value);
                    }
                }

                // Thử thực thi stored procedure để kiểm tra xem nó có tồn tại không
                try
                {
                    using var reader = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
                    var schemaTable = reader.GetSchemaTable();
                    
                    if (schemaTable == null || schemaTable.Rows.Count == 0)
                    {
                        _logger?.LogWarning($"Stored procedure '{procedureName}' không trả về schema nào.");
                        return new DataTable(); // Trả về bảng trống
                    }
                    
                    return schemaTable;
                }
                catch (SqlException sqlEx)
                {
                    _logger?.LogError($"Lỗi SQL khi thực thi stored procedure '{procedureName}': {sqlEx.Message}");
                    throw new InvalidOperationException($"Lỗi khi thực thi stored procedure '{procedureName}': {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Lỗi khi kết nối hoặc thực thi stored procedure '{procedureName}': {ex.Message}");
                throw new InvalidOperationException($"Không thể lấy schema cho stored procedure '{procedureName}'. Chi tiết lỗi: {ex.Message}", ex);
            }
        }
    }
} 