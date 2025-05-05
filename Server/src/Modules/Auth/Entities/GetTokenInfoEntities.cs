using System;

namespace AuthModule.DTOs;

public class GetTokenInfoEntities
{
    public int Id { get; set; }
    public bool EmployeeIsActive { get; set; }
    public string Email { get; set; }
    public int CompanyId { get; set; }    
    public bool CompanyIsActive { get; set; }
    public int Role { get; set; }    
    public string JwtID { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByIp { get; set; }
    public string LastLogin { get; set; }
} 