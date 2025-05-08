namespace AuthModule.Entities;

public class LoginResultEntities
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }   
    public int Role { get; set; }
    public bool IsActive { get; set; }
    public string PasswordHash { get; set; }    
    public int IsNewUser { get; set; }
    public int NeedSetPassword { get; set; }
    public int NeedSetCompany { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int TotalCompany { get; set; }
    public string CompanyName { get; set; }
    public string Alias { get; set; }
    public string Prefix { get; set; }
    public DateTime? CreateDate { get; set; }
    public int TotalEmployees { get; set; }
    public bool CompanyIsActive { get; set; }
}
