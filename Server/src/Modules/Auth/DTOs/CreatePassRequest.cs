using System.ComponentModel.DataAnnotations;

namespace AuthModule.DTOs;

public class CreatePassRequest
{
    [Required]
    public int EmployeeAccountMapId { get; set; }

    [Required]
    public string NewPass { get; set; }

    [Required]
    public string ComfirmPass { get; set; }
} 