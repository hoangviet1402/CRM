using System;

namespace AuthModule.Entities;

public class CompanyAccountMapEntities
{
    public int EmployeeAccountMapId { get; set; }
    public int AccountId { get; set; }
    public int CompanyId { get; set; }
    public int EmployeesInfoId { get; set; }
    public string EmployeesFullName { get; set; }    
    public int Role { get; set; }
    public bool IsActive { get; set; }
    public string PasswordHash { get; set; }
    public bool IsNewUser { get; set; }
    public bool NeedSetPassword { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Company related properties
    public string CompanyFullName { get; set; }
    public string CompanyAlias { get; set; }
    public string CompanyPrefix { get; set; }
    public DateTime CompanyCreateDate { get; set; }
    public int TotalEmployees { get; set; }
    public bool CompanyIsActive { get; set; }
    public int CreateStep { get; set; }
} 