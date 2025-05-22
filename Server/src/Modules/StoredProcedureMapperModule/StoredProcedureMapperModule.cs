using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace StoredProcedureMapperModule
{
    public class StoredProcedureMapperModule
    {
        public bool ExecuteStoredProcedure(IDbConnection connection, string procedureName, Dictionary<string, object>? parameters = null)
        {
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedureName;

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

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DataTable? ExecuteStoredProcedureWithResult(IDbConnection connection, string procedureName, Dictionary<string, object>? parameters = null)
        {
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedureName;

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

                    using (var reader = command.ExecuteReader())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DataTable?> ExecuteStoredProcedureWithResultAsync(IDbConnection connection, string procedureName, Dictionary<string, object>? parameters = null)
        {
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedureName;

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

                    using (var reader = await ((SqlCommand)command).ExecuteReaderAsync())
                    {
                        var dataTable = new DataTable();
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
} 