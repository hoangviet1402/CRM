namespace Company.Entities;

public class EmployeeCompany
{
    public int Id { get; set; }
    public int EmployeesAccountId { get; set; }
    public int CompanyId { get; set; }
    public string FullName { get; set; }
    public string Alias { get; set; }
    public string Prefix { get; set; }
    public bool IsActive { get; set; }
} 