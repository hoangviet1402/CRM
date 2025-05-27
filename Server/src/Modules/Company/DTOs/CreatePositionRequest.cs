namespace Company.DTOs;

public class CreatePositionRequest
{
    public string Name { get; set; }
    public int DepartmentId { get; set; }
    public int CompanyId { get; set; }
} 