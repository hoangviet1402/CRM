using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Employees",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                
                // Basic Information
                FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                BirthDate = table.Column<DateTime>(type: "datetime", nullable: true),
                Gender = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                EmployeeCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),

                // Contact Information
                Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                ContactAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Skype = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Facebook = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),

                // Emergency Contact
                EmergencyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                EmergencyMobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                EmergencyLandline = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                EmergencyRelation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                EmergencyAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),

                // Address Information
                Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Ward = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                PermanentAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Hometown = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                CurrentAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),

                // Identity Information
                IdentityCard = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                IdentityCardCreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                IdentityCardPlace = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                PassportID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                PassporCreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                PassporExp = table.Column<DateTime>(type: "datetime", nullable: true),
                PassporPlace = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),

                // Bank Information
                BankHolder = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                BankAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                BankBranch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),

                // Other Information
                TaxIdentification = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Employees", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Employees");
    }
} 