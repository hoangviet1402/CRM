using Microsoft.AspNetCore.Identity;

namespace AuthModule.Entities;

public class LoginEntities 
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public int? CompanyId { get; set; }
    public bool? CompanyIsActive { get; set; }
    public int? Role { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool? IsActive { get; set; }
} 