namespace AuthModule.DTOs;

public class UpdateFullNameResponse
{
        public int? EmployeeAccountMapID { get; set; }
        public int? CompanyId { get; set; }
        public int? AccountID { get; set; }
}

public class UpdateFullNameResquest
{
        public string? Phone { get; set; }
        public string? Mail { get; set; }
        public string FullName { get; set; }
}

