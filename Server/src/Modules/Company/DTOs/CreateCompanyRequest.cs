using System.ComponentModel.DataAnnotations;

namespace Company.DTOs;

public class CreateCompanyRequest
{
    [Required(ErrorMessage = "Tên công ty là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên công ty không được vượt quá 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(250, ErrorMessage = "Địa chỉ không được vượt quá 250 ký tự")]
    public string Address { get; set; } = string.Empty;
} 