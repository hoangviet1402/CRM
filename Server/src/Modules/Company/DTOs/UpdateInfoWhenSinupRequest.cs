using Newtonsoft.Json;

namespace Company.DTOs;

public class UpdateInfoWhenSinupRequest
{
    [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
    public int AccountId { get; set; }

    [JsonProperty("shop", NullValueHandling = NullValueHandling.Ignore)]
    public int CompanyId { get; set; }

    [JsonProperty("shop_name", NullValueHandling = NullValueHandling.Ignore)]
    public string CompanyName { get; set; }

    [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
    public int CompanyLatitude { get; set; }

    [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
    public int CompanyLongitude { get; set; }

    [JsonProperty("number_employee_expected", NullValueHandling = NullValueHandling.Ignore)]
    public string CompanyNumberEmploye { get; set; }

    [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
    public string CompanyAddress { get; set; }

    [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
    public string Email { get; set; }

    [JsonProperty("hear_about_tantam", NullValueHandling = NullValueHandling.Ignore)]
    public List<string> HearAbout { get; set; }

    [JsonProperty("use_purpose", NullValueHandling = NullValueHandling.Ignore)]
    public List<string> UsePurpose { get; set; }
}