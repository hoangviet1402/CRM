using System;

namespace AuthModule.Entities;

public class AccountTokenInfoEntities
{
    public int Id { get; set; }
    public int EmployeeAccountMapId { get; set; }
    public string JwtID { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
    public string Ip { get; set; }
    public bool IsActive { get; set; }
    public int Role { get; set; }
    public int AccountId { get; set; }
    public int CompanyId { get; set; }
    public int EmployeesInfoId { get; set; }
    public bool CompanyIsActive { get; set; }
    public bool AccountIsActive { get; set; }
} 