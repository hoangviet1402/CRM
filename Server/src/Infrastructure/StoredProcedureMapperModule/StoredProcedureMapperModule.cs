using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Infrastructure.StoredProcedureMapperModule;

public class StoredProcedureMapperModule
{
    public async Task<bool> ExecuteStoredProcedureAsync(DbConnection connection, string storedProcedureName, Dictionary<string, object> parameters = null, Dictionary<string, object> outputParameters = null)
    {
        bool shouldCloseConnection = false;
        try
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
                shouldCloseConnection = true;
            }

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;

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
                    command.Parameters.Add(parameter);
                }
            }

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

            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            if (shouldCloseConnection && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }

    public async Task<DataTable> ExecuteStoredProcedureWithResultAsync(DbConnection connection, string storedProcedureName, Dictionary<string, object> parameters = null)
    {
        bool shouldCloseConnection = false;
        try
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
                shouldCloseConnection = true;
            }

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;

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

            var dataTable = new DataTable();
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);
            return dataTable;
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            if (shouldCloseConnection && connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }
        }
    }
} 