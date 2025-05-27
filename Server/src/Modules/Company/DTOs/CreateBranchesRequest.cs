namespace Company.DTOs;

public class CreateBranchesRequest
{
    public int CompanyId { get; set; }
    public List<string> BranchNames { get; set; }
} 