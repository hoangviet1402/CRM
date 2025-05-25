using System;
using System.Collections.Generic;

namespace SqlMapper.Models
{
    public class Ins_Account_GetAllCompany_Result
    {
        public int EmployeeAccountMapId { get; set; }
        public int AccountId { get; set; }
        public int? EmployeesInfoId { get; set; }
        public string EmployeesFullName { get; set; }
        public int Role { get; set; }
        public bool IsActive { get; set; }
        public string PasswordHash { get; set; }
        public int IsNewUser { get; set; }
        public int NeedSetPassword { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyFullName { get; set; }
        public string Alias { get; set; }
        public string Prefix { get; set; }
        public DateTime? CompanyCreateDate { get; set; }
        public int? TotalEmployees { get; set; }
        public bool CompanyIsActive { get; set; }
        public int CreateStep { get; set; }
    }
}
