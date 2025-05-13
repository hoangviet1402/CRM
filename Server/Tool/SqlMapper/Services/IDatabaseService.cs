using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SqlMapper.Services
{
    public interface IDatabaseService
    {
        /// <summary>
        /// Executes a stored procedure and returns a list of results.
        /// </summary>
        /// <typeparam name="T">The type to map the results to.</typeparam>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters for the stored procedure.</param>
        /// <returns>A list of results mapped to the specified type.</returns>
        Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string procedureName, object parameters = null);

        /// <summary>
        /// Executes a stored procedure and returns a single result.
        /// </summary>
        /// <typeparam name="T">The type to map the result to.</typeparam>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters for the stored procedure.</param>
        /// <returns>A single result mapped to the specified type.</returns>
        Task<T> QueryStoredProcedureSingleAsync<T>(string procedureName, object parameters = null);

        /// <summary>
        /// Executes a stored procedure that does not return a result.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters for the stored procedure.</param>
        /// <returns>The number of rows affected.</returns>
        Task<int> ExecuteStoredProcedureAsync(string procedureName, object parameters = null);

        /// <summary>
        /// Gets the schema information for a stored procedure's result set.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="parameters">The parameters for the stored procedure.</param>
        /// <returns>A DataTable containing the schema information.</returns>
        Task<DataTable> GetStoredProcedureSchemaAsync(string procedureName, object parameters = null);
    }
} 