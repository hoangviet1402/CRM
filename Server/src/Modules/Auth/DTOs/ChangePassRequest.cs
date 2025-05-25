using System.ComponentModel.DataAnnotations;

namespace AuthModule.DTOs;

public class ChangePassRequest
{
    [Required]
    public int EmployeeAccountMapId { get; set; }

    [Required]
    public string NewPass { get; set; }

    [Required]
    public string OldPass { get; set; }
} 