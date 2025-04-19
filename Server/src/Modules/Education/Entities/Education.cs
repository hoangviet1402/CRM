using Shared.Entities;

namespace Education.Entities;

public class Education
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Degree { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public decimal Grade { get; set; }
    public string Location { get; set; } = string.Empty;
} 