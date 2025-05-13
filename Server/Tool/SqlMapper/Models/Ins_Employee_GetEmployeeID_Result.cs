using System;
using System.Collections.Generic;

namespace SqlMapper.Models
{
    public class Ins_Employee_GetEmployeeID_Result
    {
    /*
     * CẢNH BÁO: Phát hiện cột trùng tên trong kết quả stored procedure.
     * Các cột trùng tên: CreatedAt
     * Chỉ thuộc tính cuối cùng với mỗi tên sẽ được giữ lại trong lớp này.
     * Vui lòng kiểm tra lại stored procedure hoặc sửa tên lớp thủ công.
     */
        public int IdData { get; set; }
        public int? CompanyId { get; set; }
        public int? EmployeesInfoId { get; set; }
        public int? Role { get; set; }
        public bool? IsActive { get; set; }
        public string PasswordHash { get; set; }
        public int? IsNewUser { get; set; }
        public int? NeedSetPassword { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string FullName { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? Gender { get; set; }
        public int? DisplayOrder { get; set; }
        public string ContactAddress { get; set; }
        public string Skype { get; set; }
        public string Facebook { get; set; }
        public string EmergencyName { get; set; }
        public string EmergencyMobile { get; set; }
        public string EmergencyLandline { get; set; }
        public string EmergencyRelation { get; set; }
        public string EmergencyAddress { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string PermanentAddress { get; set; }
        public string Hometown { get; set; }
        public string CurrentAddress { get; set; }
        public string IdentityCard { get; set; }
        public DateTime? IdentityCardCreateDate { get; set; }
        public string IdentityCardPlace { get; set; }
        public string PassportID { get; set; }
        public DateTime? PassporCreateDate { get; set; }
        public DateTime? PassporExp { get; set; }
        public string PassporPlace { get; set; }
        public string BankHolder { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string TaxIdentification { get; set; }
        // Thuộc tính bị trùng tên: CreatedAt
    }
}
