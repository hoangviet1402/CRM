using Infrastructure.Entities;

namespace MajorsModule.Entities;

public class MajorsEntity : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
} 