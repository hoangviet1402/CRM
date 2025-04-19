using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace EducationsLevelModule.Entities;

public class EducationsLevelEntity : IModuleEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }
} 