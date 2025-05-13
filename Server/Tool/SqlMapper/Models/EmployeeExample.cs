using System;
using System.Collections.Generic;

namespace SqlMapper.Models
{
    /// <summary>
    /// This is an example class that would be automatically generated from 
    /// a stored procedure that returns employee data.
    /// </summary>
    public class EmployeeExample
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? HireDate { get; set; }
        public decimal? Salary { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Position { get; set; }
        public bool IsActive { get; set; }
    }
} 