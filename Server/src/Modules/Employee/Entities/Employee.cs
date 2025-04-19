using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeModule.Entities;

[Table("Employees")]
public class EmployeeEntity
{
    [Key]
    public int Id { get; set; }

    // Basic Information
    [MaxLength(100)]
    public string? FullName { get; set; }
    public DateTime? BirthDate { get; set; }
    public int Gender { get; set; } // 0: Female, 1: Male
    [MaxLength(50)]
    public string? EmployeeCode { get; set; }
    public int DisplayOrder { get; set; }

    // Contact Information
    [MaxLength(100)]
    public string? Email { get; set; }
    [MaxLength(20)]
    public string? Phone { get; set; }
    [MaxLength(200)]
    public string? ContactAddress { get; set; }
    [MaxLength(50)]
    public string? Skype { get; set; }
    [MaxLength(100)]
    public string? Facebook { get; set; }

    // Emergency Contact
    [MaxLength(100)]
    public string? EmergencyName { get; set; }
    [MaxLength(20)]
    public string? EmergencyMobile { get; set; }
    [MaxLength(20)]
    public string? EmergencyLandline { get; set; }
    [MaxLength(50)]
    public string? EmergencyRelation { get; set; }
    [MaxLength(200)]
    public string? EmergencyAddress { get; set; }

    // Address Information
    [MaxLength(100)]
    public string? Country { get; set; }
    [MaxLength(100)]
    public string? Province { get; set; }
    [MaxLength(100)]
    public string? District { get; set; }
    [MaxLength(100)]
    public string? Ward { get; set; }
    [MaxLength(200)]
    public string? PermanentAddress { get; set; }
    [MaxLength(200)]
    public string? Hometown { get; set; }
    [MaxLength(200)]
    public string? CurrentAddress { get; set; }

    // Identity Information
    [MaxLength(20)]
    public string? IdentityCard { get; set; }
    public DateTime? IdentityCardCreateDate { get; set; }
    [MaxLength(200)]
    public string? IdentityCardPlace { get; set; }
    [MaxLength(20)]
    public string? PassportID { get; set; }
    public DateTime? PassporCreateDate { get; set; }
    public DateTime? PassporExp { get; set; }
    [MaxLength(200)]
    public string? PassporPlace { get; set; }

    // Bank Information
    [MaxLength(100)]
    public string? BankHolder { get; set; }
    [MaxLength(50)]
    public string? BankAccount { get; set; }
    [MaxLength(100)]
    public string? BankName { get; set; }
    [MaxLength(100)]
    public string? BankBranch { get; set; }

    // Other Information
    [MaxLength(100)]
    public string? TaxIdentification { get; set; }
} 