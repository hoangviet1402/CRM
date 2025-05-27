namespace Company.DTOs;

public class CreateDepartmentInAllBranchesRequest
{
    public List<string> Names { get; set; }
    public int CompanyId { get; set; }
} 