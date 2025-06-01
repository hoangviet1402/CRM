namespace Company.Entities;

public class DepartmentCreatedResult
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CompanyId { get; set; }

    public int BranchId { get; set; }

    public DateTime CreatedAt { get; set; }
    // SELECT Id, @Name AS [Name], @CompanyId AS [CompanyId] ,[BranchId], [CreatedAt]  FROM @InsertedIds;
} 