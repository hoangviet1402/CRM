namespace AuthModule.Entities;


public class LoginResultEntities
{
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Phone { get; set; }
        public string? PhoneCode { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public int? EmployeesInfoId { get; set; }
        public int Role { get; set; }
        public bool IsActive { get; set; }
        public string PasswordHash { get; set; }
        public bool IsNewUser { get; set; }
        public bool NeedSetPassword { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string FullName { get; set; }

}

public class ValidateResultEntities
{
    public int AccountId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }   
    public bool IsActive { get; set; }
    public int TotalCompany { get; set; }    
    public DateTime CreatedAt { get; set; }
}

