using Newtonsoft.Json;

namespace AuthModule.DTOs;


public class AuthResponse
{
    [JsonProperty("signin_methods")]
    public List<string>? SigninMethods { get; set; }

    [JsonProperty("shop")]
    public AuthCompanyResponse? Company { get; set; }

    [JsonProperty("shops")]
    public List<AuthCompaniesResponse>? ListCompanies { get; set; }

    [JsonProperty("user")]
    public AuthUserResponse? User { get; set; }
}

public class AuthCompanyResponse
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("shop_username")]
    public string? ShopUsername { get; set; }

    [JsonProperty("user_id")]
    public int? UserId { get; set; }

    [JsonProperty("is_new_user")]
    public bool? IsNewUser { get; set; }

    [JsonProperty("need_set_password")]
    public bool? NeedSetPassword { get; set; }
}

public class AuthUserResponse
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("client_role")]
    public string? ClientRole { get; set; }

    [JsonProperty("phone")]
    public string? Phone { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }
}

public class AuthCompaniesResponse
{
    [JsonProperty("client_role")]
    public string? ClientRole { get; set; }

    [JsonProperty("employee_name")]
    public string? EmployeeName { get; set; }

    [JsonProperty("user_id")]
    public int? UserId { get; set; }

    [JsonProperty("need_set_password")]
    public bool? NeedSetPassword { get; set; }

    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("shop_username")]
    public string? ShopUsername { get; set; }

    [JsonProperty("is_new_user")]
    public bool? IsNewUser { get; set; }
}