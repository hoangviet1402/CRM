using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace SqlMapper.Utilities
{
    public class ClassGenerator
    {
        // Lớp kết quả khi tạo class, bao gồm cả cảnh báo
        public class GenerationResult 
        {
            public string ClassContent { get; set; }
            public List<string> Warnings { get; set; } = new List<string>();
            public bool HasWarnings => Warnings.Count > 0;
        }

        public static GenerationResult GenerateClassFromSchema(DataTable schemaTable, string className)
        {
            var result = new GenerationResult();
            
            if (schemaTable == null || schemaTable.Rows.Count == 0)
            {
                result.ClassContent = string.Empty;
                return result;
            }

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("namespace SqlMapper.Models");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");

            // Kiểm tra cột trùng tên
            var columnNames = new HashSet<string>();
            var duplicateColumns = new HashSet<string>();

            // Xác định các cột trùng tên
            foreach (DataRow row in schemaTable.Rows)
            {
                var columnName = row["ColumnName"].ToString();
                if (!string.IsNullOrWhiteSpace(columnName))
                {
                    if (!columnNames.Add(columnName))
                    {
                        duplicateColumns.Add(columnName);
                    }
                }
            }

            // Thêm cảnh báo nếu phát hiện cột trùng tên
            if (duplicateColumns.Count > 0)
            {
                string duplicatesMessage = $"Phát hiện {duplicateColumns.Count} cột trùng tên: {string.Join(", ", duplicateColumns)}";
                result.Warnings.Add(duplicatesMessage);
                
                // Thêm comment vào file để cảnh báo
                sb.AppendLine("    /*");
                sb.AppendLine("     * CẢNH BÁO: Phát hiện cột trùng tên trong kết quả stored procedure.");
                sb.AppendLine("     * Các cột trùng tên: " + string.Join(", ", duplicateColumns));
                sb.AppendLine("     * Chỉ thuộc tính cuối cùng với mỗi tên sẽ được giữ lại trong lớp này.");
                sb.AppendLine("     * Vui lòng kiểm tra lại stored procedure hoặc sửa tên lớp thủ công.");
                sb.AppendLine("     */");
            }

            // Theo dõi các cột đã xử lý để tránh trùng lặp
            var processedColumns = new HashSet<string>();

            foreach (DataRow row in schemaTable.Rows)
            {
                var columnName = row["ColumnName"].ToString();
                var dataType = row["DataType"] as Type;
                var allowDBNull = (bool)row["AllowDBNull"];

                // Skip columns without names
                if (string.IsNullOrWhiteSpace(columnName))
                {
                    continue;
                }

                // Nếu cột đã được xử lý (trùng tên), thêm comment
                if (processedColumns.Contains(columnName))
                {
                    sb.AppendLine($"        // Thuộc tính bị trùng tên: {columnName}");
                    continue;
                }

                // Thêm vào danh sách đã xử lý
                processedColumns.Add(columnName);

                // Convert SQL type to C# type
                var csType = GetCSharpType(dataType, allowDBNull);
                
                // Add property
                sb.AppendLine($"        public {csType} {columnName} {{ get; set; }}");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            result.ClassContent = sb.ToString();
            return result;
        }

        public static string GetCSharpType(Type sqlType, bool isNullable)
        {
            if (sqlType == null)
            {
                return "object";
            }

            var typeName = sqlType.Name.ToLower();
            string csType;

            // Map SQL types to C# types
            switch (typeName)
            {
                case "int":
                case "int32":
                    csType = "int";
                    break;
                case "bigint":
                case "int64":
                    csType = "long";
                    break;
                case "smallint":
                case "int16":
                    csType = "short";
                    break;
                case "tinyint":
                case "byte":
                    csType = "byte";
                    break;
                case "bit":
                case "boolean":
                    csType = "bool";
                    break;
                case "decimal":
                    csType = "decimal";
                    break;
                case "money":
                case "smallmoney":
                    csType = "decimal";
                    break;
                case "float":
                case "single":
                    csType = "float";
                    break;
                case "real":
                case "double":
                    csType = "double";
                    break;
                case "datetime":
                case "datetime2":
                case "date":
                case "smalldatetime":
                    csType = "DateTime";
                    break;
                case "datetimeoffset":
                    csType = "DateTimeOffset";
                    break;
                case "time":
                    csType = "TimeSpan";
                    break;
                case "char":
                case "varchar":
                case "text":
                case "nchar":
                case "nvarchar":
                case "ntext":
                case "string":
                    csType = "string";
                    break;
                case "binary":
                case "varbinary":
                case "image":
                    csType = "byte[]";
                    break;
                case "uniqueidentifier":
                    csType = "Guid";
                    break;
                default:
                    csType = "object";
                    break;
            }

            // Handle nullable types
            if (isNullable && csType != "string" && csType != "byte[]" && csType != "object")
            {
                csType += "?";
            }

            return csType;
        }

        public static void SaveClassToFile(string classContent, string className, string outputDirectory)
        {
            if (string.IsNullOrWhiteSpace(classContent))
            {
                return;
            }

            // Ensure the directory exists
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Create file path
            var filePath = Path.Combine(outputDirectory, $"{className}.cs");

            // Write content to file
            File.WriteAllText(filePath, classContent);
        }
    }
} 