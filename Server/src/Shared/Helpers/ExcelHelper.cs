using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Drawing;

namespace Shared.Helpers
{
    public static class ExcelHelper
    {
        static ExcelHelper()
        {
            // Set license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Đọc file Excel thành DataTable
        /// </summary>
        /// <param name="filePath">Đường dẫn file Excel</param>
        /// <param name="sheetName">Tên sheet (nếu null sẽ đọc sheet đầu tiên)</param>
        /// <param name="hasHeader">Có header hay không</param>
        /// <returns>DataTable chứa dữ liệu</returns>
        public static DataTable ReadExcelToDataTable(string filePath, string? sheetName = null, bool hasHeader = true)
        {
            var dt = new DataTable();
            
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = sheetName == null 
                    ? package.Workbook.Worksheets[0] 
                    : package.Workbook.Worksheets[sheetName];

                if (worksheet == null)
                    throw new Exception($"Sheet '{sheetName}' không tồn tại trong file Excel.");

                int startRow = hasHeader ? 2 : 1;
                int colCount = worksheet.Dimension.Columns;
                int rowCount = worksheet.Dimension.Rows;

                // Add columns
                for (int col = 1; col <= colCount; col++)
                {
                    string columnName = hasHeader 
                        ? worksheet.Cells[1, col].Value?.ToString() ?? $"Column{col}"
                        : $"Column{col}";
                    dt.Columns.Add(columnName);
                }

                // Add rows
                for (int row = startRow; row <= rowCount; row++)
                {
                    var dataRow = dt.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Value?.ToString() ?? "";
                    }
                    dt.Rows.Add(dataRow);
                }
            }

            return dt;
        }

        /// <summary>
        /// Xuất DataTable ra file Excel
        /// </summary>
        /// <param name="dataTable">DataTable cần xuất</param>
        /// <param name="filePath">Đường dẫn file Excel</param>
        /// <param name="sheetName">Tên sheet</param>
        public static void ExportDataTableToExcel(DataTable dataTable, string filePath, string sheetName = "Sheet1")
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                // Add headers
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Add data
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = dataTable.Rows[row][col];
                    }
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Save file
                package.SaveAs(new FileInfo(filePath));
            }
        }

        /// <summary>
        /// Xuất danh sách đối tượng ra file Excel
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng</typeparam>
        /// <param name="data">Danh sách đối tượng</param>
        /// <param name="filePath">Đường dẫn file Excel</param>
        /// <param name="sheetName">Tên sheet</param>
        public static void ExportListToExcel<T>(IEnumerable<T> data, string filePath, string sheetName = "Sheet1")
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                var properties = typeof(T).GetProperties();

                // Add headers
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Add data
                int row = 2;
                foreach (var item in data)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        worksheet.Cells[row, col + 1].Value = properties[col].GetValue(item)?.ToString() ?? "";
                    }
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Save file
                package.SaveAs(new FileInfo(filePath));
            }
        }

        /// <summary>
        /// Đọc file Excel thành danh sách đối tượng
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của đối tượng</typeparam>
        /// <param name="filePath">Đường dẫn file Excel</param>
        /// <param name="sheetName">Tên sheet (nếu null sẽ đọc sheet đầu tiên)</param>
        /// <returns>Danh sách đối tượng</returns>
        public static List<T> ReadExcelToList<T>(string filePath, string sheetName = null) where T : new()
        {
            var result = new List<T>();
            var properties = typeof(T).GetProperties();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = sheetName == null 
                    ? package.Workbook.Worksheets[0] 
                    : package.Workbook.Worksheets[sheetName];

                if (worksheet == null)
                    throw new Exception($"Sheet '{sheetName}' không tồn tại trong file Excel.");

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Get header row
                var headers = new Dictionary<string, int>();
                for (int col = 1; col <= colCount; col++)
                {
                    var header = worksheet.Cells[1, col].Value?.ToString();
                    if (!string.IsNullOrEmpty(header))
                        headers[header] = col;
                }

                // Read data rows
                for (int row = 2; row <= rowCount; row++)
                {
                    var item = new T();
                    foreach (var prop in properties)
                    {
                        if (headers.TryGetValue(prop.Name, out int col))
                        {
                            var value = worksheet.Cells[row, col].Value;
                            if (value != null)
                            {
                                try
                                {
                                    var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                                    prop.SetValue(item, convertedValue);
                                }
                                catch
                                {
                                    // Skip if conversion fails
                                }
                            }
                        }
                    }
                    result.Add(item);
                }
            }

            return result;
        }
    }
} 