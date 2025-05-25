using System.Numerics;
using AutoMapper.Configuration.Annotations;
using Newtonsoft.Json;

namespace AuthModule.DTOs;

public class AuthResponse
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? AccessToken { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? RefreshToken { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Model { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public AuthUserResponse? User { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public AuthCompanyResponse? Company { get; set; }

    public List<AuthCompanyResponse>? ListCompanies { get; set; }
}

public class AuthUserResponse
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? EmployeesInfoId { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

    public string? FullName { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Phone { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Email { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsNewUser { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsActive { get; set; }
}

public class AuthCompanyResponse
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string FullName { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool IsActive { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int Role { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool NeedSetPassword { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool NeedSetCompany { get; set; }
    public int? CreateStep { get; set; }
}