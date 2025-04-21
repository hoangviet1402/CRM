using Microsoft.AspNetCore.Identity;

namespace AuthModule.Entities;

public class ApplicationUser 
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public int? Role { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; } = true;
} 