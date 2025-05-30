using Newtonsoft.Json;

namespace AuthModule.DTOs;
public class AuthResponse
{
    [JsonProperty("signin_methods", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? SigninMethods { get; set; }

    [JsonProperty("shop", NullValueHandling = NullValueHandling.Ignore)]
    public AuthCompanyResponse? Company { get; set; }

    [JsonProperty("shops", NullValueHandling = NullValueHandling.Ignore)]
    public List<AuthCompaniesResponse>? ListCompanies { get; set; }

    [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
    public AuthUserResponse? User { get; set; }
}

public class AuthCompanyResponse
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("shop_username", NullValueHandling = NullValueHandling.Ignore)]
    public string? ShopUsername { get; set; }

    [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
    public int? UserId { get; set; }

    [JsonProperty("is_new_user", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsNewUser { get; set; }

    [JsonProperty("need_set_password", NullValueHandling = NullValueHandling.Ignore)]
    public bool? NeedSetPassword { get; set; }
}

public class AuthUserResponse
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("client_role", NullValueHandling = NullValueHandling.Ignore)]
    public string? ClientRole { get; set; }

    [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
    public string? Phone { get; set; }

    [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
    public string? Email { get; set; }
}

public class AuthCompaniesResponse
{
    [JsonProperty("client_role", NullValueHandling = NullValueHandling.Ignore)]
    public string? ClientRole { get; set; }

    [JsonProperty("employee_name", NullValueHandling = NullValueHandling.Ignore)]
    public string? EmployeeName { get; set; }

    [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
    public int? UserId { get; set; }

    [JsonProperty("need_set_password", NullValueHandling = NullValueHandling.Ignore)]
    public bool? NeedSetPassword { get; set; }

    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("shop_username", NullValueHandling = NullValueHandling.Ignore)]
    public string? ShopUsername { get; set; }

    [JsonProperty("is_new_user", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsNewUser { get; set; }
}

public class ValidateAccountRequest
{
    [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
    public string? Phone { get; set; }

    [JsonProperty("phone_code", NullValueHandling = NullValueHandling.Ignore)]
    public string? PhoneCode { get; set; }

    [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
    public string? Email { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("fullname", NullValueHandling = NullValueHandling.Ignore)]
    public string? Fullname { get; set; }

    [JsonProperty("provider", NullValueHandling = NullValueHandling.Ignore)]
    public string? Provider { get; set; }

    [JsonProperty("is_no_otp_flow", NullValueHandling = NullValueHandling.Ignore)]
    public int? IsNoOtpFlow { get; set; }
}