using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlMapper.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace SqlMapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelGeneratorController : ControllerBase
    {
        private readonly IModelGeneratorService _modelGeneratorService;
        private readonly ILogger<ModelGeneratorController> _logger;
        private readonly ModelGeneratorService _concreteService;

        public ModelGeneratorController(IModelGeneratorService modelGeneratorService, ILogger<ModelGeneratorController> logger)
        {
            _modelGeneratorService = modelGeneratorService ?? throw new ArgumentNullException(nameof(modelGeneratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _concreteService = modelGeneratorService as ModelGeneratorService;
        }

        // Hàm helper để tạo tên lớp từ tên stored procedure
        private string GenerateClassName(string procedureName)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
                return null;

            // Lấy phần cuối cùng sau dấu chấm hoặc toàn bộ tên nếu không có dấu chấm
            string baseName = procedureName.Contains(".") 
                ? procedureName.Substring(procedureName.LastIndexOf('.') + 1) 
                : procedureName;
                
            // Thêm hậu tố "_Result"
            return baseName + "_Result";
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateModel(
            [FromQuery] string procedureName,
            [FromQuery] string className = null,
            [FromQuery] bool download = false)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                return BadRequest("Tên stored procedure là bắt buộc");
            }

            // Nếu không cung cấp tên lớp, tự động tạo từ tên stored procedure
            className = string.IsNullOrWhiteSpace(className) 
                ? GenerateClassName(procedureName) 
                : className;

            try
            {
                _logger.LogInformation($"Đang tạo mô hình từ stored procedure: {procedureName}, tên lớp: {className}");
                
                if (_concreteService != null)
                {
                    // Sử dụng phiên bản cụ thể nếu có
                    var result = await _concreteService.GenerateClassFromStoredProcedureAsync(procedureName, className);
                    
                    if (string.IsNullOrWhiteSpace(result.Content))
                    {
                        _logger.LogWarning($"Không tìm thấy schema cho stored procedure: {procedureName}");
                        return NotFound($"Không tìm thấy schema cho stored procedure '{procedureName}'. Kiểm tra xem stored procedure có tồn tại và trả về dữ liệu không.");
                    }

                    _logger.LogInformation($"Đã tạo thành công mô hình cho stored procedure: {procedureName}");

                    // Nếu có yêu cầu download file
                    if (download)
                    {
                        // Lưu file tạm thời
                        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), className + ".cs");
                        await System.IO.File.WriteAllTextAsync(tempPath, result.Content);
                        var fileBytes = await System.IO.File.ReadAllBytesAsync(tempPath);
                        var fileName = className + ".cs";
                        return File(fileBytes, "text/plain", fileName);
                    }
                    // Trả về nội dung lớp và cảnh báo (nếu có)
                    return Ok(new {
                        ClassContent = result.Content,
                        HasWarnings = result.HasWarnings,
                        Warnings = result.Warnings
                    });
                }
                else
                {
                    // Sử dụng giao diện nếu không thể cast
                    var classContent = await _modelGeneratorService.GenerateClassFromStoredProcedureAsync(procedureName, className);
                    
                    if (string.IsNullOrWhiteSpace(classContent))
                    {
                        _logger.LogWarning($"Không tìm thấy schema cho stored procedure: {procedureName}");
                        return NotFound($"Không tìm thấy schema cho stored procedure '{procedureName}'. Kiểm tra xem stored procedure có tồn tại và trả về dữ liệu không.");
                    }

                    _logger.LogInformation($"Đã tạo thành công mô hình cho stored procedure: {procedureName}");

                    if (download)
                    {
                        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), className + ".cs");
                        await System.IO.File.WriteAllTextAsync(tempPath, classContent);
                        var fileBytes = await System.IO.File.ReadAllBytesAsync(tempPath);
                        var fileName = className + ".cs";
                        return File(fileBytes, "text/plain", fileName);
                    }
                    return Ok(classContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi tạo mô hình từ stored procedure '{procedureName}': {ex.Message}");
                
                // Trả về thông tin lỗi chi tiết
                return StatusCode(500, new 
                { 
                    Error = $"Lỗi khi tạo mô hình từ stored procedure '{procedureName}'",
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateAndSaveModel(
            [FromQuery] string procedureName,
            [FromQuery] string className = null,
            [FromQuery] bool download = false)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                return BadRequest("Tên stored procedure là bắt buộc");
            }

            // Nếu không cung cấp tên lớp, tự động tạo từ tên stored procedure
            className = string.IsNullOrWhiteSpace(className) 
                ? GenerateClassName(procedureName) 
                : className;

            try
            {
                _logger.LogInformation($"Đang tạo và lưu mô hình từ stored procedure: {procedureName}, tên lớp: {className}");
                
                if (_concreteService != null)
                {
                    // Sử dụng phiên bản cụ thể nếu có
                    var result = await _concreteService.GenerateAndSaveClassAsync(procedureName, className);
                    
                    _logger.LogInformation($"Đã tạo và lưu thành công mô hình cho stored procedure: {procedureName}, đường dẫn: {result.FilePath}");

                    if (download && !string.IsNullOrWhiteSpace(result.FilePath) && System.IO.File.Exists(result.FilePath))
                    {
                        var fileBytes = await System.IO.File.ReadAllBytesAsync(result.FilePath);
                        var fileName = className + ".cs";
                        return File(fileBytes, "text/plain", fileName);
                    }
                    
                    return Ok(new { 
                        FilePath = result.FilePath, 
                        Message = $"Model '{className}' đã được tạo thành công",
                        HasWarnings = result.HasWarnings,
                        Warnings = result.Warnings
                    });
                }
                else
                {
                    // Sử dụng giao diện nếu không thể cast
                    var filePath = await _modelGeneratorService.GenerateAndSaveClassAsync(procedureName, className);
                    
                    _logger.LogInformation($"Đã tạo và lưu thành công mô hình cho stored procedure: {procedureName}, đường dẫn: {filePath}");

                    if (download && !string.IsNullOrWhiteSpace(filePath) && System.IO.File.Exists(filePath))
                    {
                        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                        var fileName = className + ".cs";
                        return File(fileBytes, "text/plain", fileName);
                    }
                    return Ok(new { FilePath = filePath, Message = $"Model '{className}' đã được tạo thành công" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi tạo và lưu mô hình từ stored procedure '{procedureName}': {ex.Message}");
                
                // Trả về thông tin lỗi chi tiết
                return StatusCode(500, new 
                { 
                    Error = $"Lỗi khi tạo và lưu mô hình từ stored procedure '{procedureName}'",
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
    }
} 