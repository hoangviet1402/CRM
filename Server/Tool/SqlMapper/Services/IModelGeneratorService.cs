using System.Threading.Tasks;

namespace SqlMapper.Services
{
    public interface IModelGeneratorService
    {
        /// <summary>
        /// Generates a class from a stored procedure result set.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="className">The name of the class to generate.</param>
        /// <param name="parameters">Optional parameters for the stored procedure.</param>
        /// <returns>The generated class content as a string.</returns>
        Task<string> GenerateClassFromStoredProcedureAsync(string procedureName, string className, object parameters = null);

        /// <summary>
        /// Generates a class file from a stored procedure result set and saves it to the Models directory.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure.</param>
        /// <param name="className">The name of the class to generate.</param>
        /// <param name="parameters">Optional parameters for the stored procedure.</param>
        /// <returns>The path to the generated class file.</returns>
        Task<string> GenerateAndSaveClassAsync(string procedureName, string className, object parameters = null);
    }
} 