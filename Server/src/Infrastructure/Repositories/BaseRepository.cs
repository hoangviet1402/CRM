using Infrastructure.DbContext;
using Infrastructure.StoredProcedureMapperModule;
using Microsoft.Data.SqlClient;
using System.Data;
using Shared.Extensions;
using Shared.Helpers;

namespace Infrastructure.Repositories;

public abstract class BaseRepository
{
    protected readonly DatabaseConnection _dbConnection;
    protected readonly StoredProcedureMapperModule.StoredProcedureMapperModule _storedProcedureMapper;
    protected readonly string _connectionName;

    protected BaseRepository(DatabaseConnection dbConnection, string connectionName)
    {
        _dbConnection = dbConnection;
        _storedProcedureMapper = new StoredProcedureMapperModule.StoredProcedureMapperModule();
        _connectionName = connectionName;
    }

    protected SqlConnection CreateConnection()
    {
        return _dbConnection.CreateConnection(_connectionName);
    }

    protected async Task<T> ExecuteWithConnection<T>(Func<SqlConnection, Task<T>> action)
    {
        using var connection = CreateConnection();
        return await action(connection);
    }

    protected async Task<int> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object> parameters)
    {
        return await ExecuteWithConnection(async connection =>
        {
            var outputParameters = new Dictionary<string, object>();
            var success = await _storedProcedureMapper.ExecuteStoredProcedureAsync(connection, procedureName, parameters, outputParameters);
            return success ? outputParameters.GetSafeInt32("@OutResult") : 0;
        });
    }

    protected async Task<DataTable?> ExecuteStoredProcedureWithResultAsync(string procedureName, Dictionary<string, object> parameters)
    {
        return await ExecuteWithConnection(async connection =>
        {
            return await _storedProcedureMapper.ExecuteStoredProcedureWithResultAsync(connection, procedureName, parameters);
        });
    }
} 