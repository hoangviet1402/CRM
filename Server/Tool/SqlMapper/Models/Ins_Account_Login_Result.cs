using System;
using System.Collections.Generic;

namespace SqlMapper.Models
{
    public class Ins_Account_Login_Result
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int CompanyId { get; set; }
        public int? EmployeesInfoId { get; set; }
        public int Role { get; set; }
        public bool IsActive { get; set; }
        public string PasswordHash { get; set; }
        public bool IsNewUser { get; set; }
        public bool NeedSetPassword { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string FullName { get; set; }
    }
}
