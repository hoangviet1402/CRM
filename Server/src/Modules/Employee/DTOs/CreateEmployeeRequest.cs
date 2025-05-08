using Newtonsoft.Json;

namespace EmployeeModule.DTOs;

public class CreateEmployeeRequest
{
    //[JsonProperty("fname")]
    public string FullName { get; set; } = string.Empty;
    //[JsonProperty("ecode")]
    public string EmployeeCode { get; set; } = string.Empty;
    //[JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
    //[JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;
    //[JsonProperty("pass")]
    public string Password { get; set; } = string.Empty;
    //[JsonProperty("comid")]
    public int CompanyId { get; set; } = 0;
    //[JsonProperty("role")]
    public int Role { get; set; } = 0;
}