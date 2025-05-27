namespace Company.DTOs;

public class CreateDepartmentRequest
{
    public string Name { get; set; }
    public int BranchId { get; set; }
    public int CompanyId { get; set; }
} 