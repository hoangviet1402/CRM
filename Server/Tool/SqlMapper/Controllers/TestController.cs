using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SqlMapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TestController> _logger;

        public TestController(IConfiguration configuration, ILogger<TestController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return BadRequest("Không tìm thấy chuỗi kết nối DefaultConnection trong cấu hình.");
                }

                _logger.LogInformation($"Đang kiểm tra kết nối với chuỗi kết nối: {connectionString}");
                
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    _logger.LogInformation("Kết nối đến SQL Server thành công!");
                    
                    return Ok(new 
                    { 
                        Success = true, 
                        Message = "Kết nối đến SQL Server thành công!",
                        ServerVersion = connection.ServerVersion,
                        Database = connection.Database
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kết nối đến SQL Server");
                return StatusCode(500, new 
                { 
                    Success = false, 
                    Error = "Lỗi khi kết nối đến SQL Server", 
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("procedures")]
        public async Task<IActionResult> ListStoredProcedures(string database = null)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return BadRequest("Không tìm thấy chuỗi kết nối DefaultConnection trong cấu hình.");
                }

                // Sử dụng database được chỉ định hoặc từ chuỗi kết nối
                var builder = new SqlConnectionStringBuilder(connectionString);
                if (!string.IsNullOrEmpty(database))
                {
                    builder.InitialCatalog = database;
                }

                _logger.LogInformation($"Đang liệt kê stored procedures trong database: {builder.InitialCatalog}");
                
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync();
                    
                    using (var command = new SqlCommand(@"
                        SELECT 
                            ROUTINE_SCHEMA + '.' + ROUTINE_NAME AS ProcedureName,
                            CREATED AS CreatedDate,
                            LAST_ALTERED AS LastModifiedDate
                        FROM 
                            INFORMATION_SCHEMA.ROUTINES
                        WHERE 
                            ROUTINE_TYPE = 'PROCEDURE'
                        ORDER BY 
                            ROUTINE_NAME", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var procedures = new System.Collections.Generic.List<object>();
                            
                            while (await reader.ReadAsync())
                            {
                                procedures.Add(new
                                {
                                    Name = reader.GetString(0),
                                    Created = reader.GetDateTime(1),
                                    Modified = reader.GetDateTime(2)
                                });
                            }
                            
                            return Ok(new 
                            { 
                                Success = true, 
                                Database = builder.InitialCatalog,
                                ProcedureCount = procedures.Count,
                                Procedures = procedures
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi liệt kê stored procedures");
                return StatusCode(500, new 
                { 
                    Success = false, 
                    Error = "Lỗi khi liệt kê stored procedures", 
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
    }
} 