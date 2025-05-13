using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SqlMapper.Utilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SqlMapper.Services
{
    public class ModelGeneratorService : IModelGeneratorService
    {
        private readonly IDatabaseService _databaseService;
        private readonly string _modelsDirectory;
        private readonly ILogger<ModelGeneratorService> _logger;

        public class ModelGenerationResult
        {
            public string Content { get; set; }
            public string FilePath { get; set; }
            public List<string> Warnings { get; set; } = new List<string>();
            public bool HasWarnings => Warnings.Count > 0;
        }

        public ModelGeneratorService(IDatabaseService databaseService, IHostEnvironment hostEnvironment, ILogger<ModelGeneratorService> logger = null)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _modelsDirectory = Path.Combine(hostEnvironment.ContentRootPath, "Models");
            _logger = logger;
        }

        public async Task<ModelGenerationResult> GenerateClassFromStoredProcedureAsync(string procedureName, string className, object parameters = null)
        {
            // Get the schema information from the stored procedure
            var schemaTable = await _databaseService.GetStoredProcedureSchemaAsync(procedureName, parameters);
            
            // Generate the class
            var result = ClassGenerator.GenerateClassFromSchema(schemaTable, className);
            
            if (result.HasWarnings)
            {
                foreach (var warning in result.Warnings)
                {
                    _logger?.LogWarning($"Cảnh báo khi tạo lớp {className} từ {procedureName}: {warning}");
                }
            }
            
            return new ModelGenerationResult 
            {
                Content = result.ClassContent,
                Warnings = result.Warnings
            };
        }

        public async Task<ModelGenerationResult> GenerateAndSaveClassAsync(string procedureName, string className, object parameters = null)
        {
            // Generate the class content
            var result = await GenerateClassFromStoredProcedureAsync(procedureName, className, parameters);

            if (string.IsNullOrWhiteSpace(result.Content))
            {
                throw new InvalidOperationException($"No schema found for stored procedure '{procedureName}'");
            }

            // Save the class to a file
            var filePath = Path.Combine(_modelsDirectory, $"{className}.cs");
            ClassGenerator.SaveClassToFile(result.Content, className, _modelsDirectory);

            result.FilePath = filePath;
            return result;
        }

        // Triển khai phương thức từ giao diện ban đầu để tương thích ngược
        async Task<string> IModelGeneratorService.GenerateClassFromStoredProcedureAsync(string procedureName, string className, object parameters)
        {
            var result = await GenerateClassFromStoredProcedureAsync(procedureName, className, parameters);
            return result.Content;
        }

        async Task<string> IModelGeneratorService.GenerateAndSaveClassAsync(string procedureName, string className, object parameters)
        {
            var result = await GenerateAndSaveClassAsync(procedureName, className, parameters);
            return result.FilePath;
        }
    }
} 