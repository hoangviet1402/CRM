using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AuthModule.DTOs;

public class AppInfo
{
    [JsonProperty("package_name")]
    public string PackageName { get; set; }
}

public class SignupRequest
{
    [JsonProperty("phone_code")]
    public string PhoneCode { get; set; }

    [JsonProperty("mail")]
    public string Mail { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("fullname")]
    public string Fullname { get; set; }

    [JsonProperty("stage")]
    public string Stage { get; set; }

    [JsonProperty("provider")]
    public string Provider { get; set; }

    [JsonProperty("device_id")]
    public string DeviceId { get; set; }

    [JsonProperty("appInfo")]
    public AppInfo AppInfo { get; set; }

    [JsonProperty("ignoreToken")]
    public bool IgnoreToken { get; set; }

    [JsonProperty("is_mobile_menu")]
    public int IsMobileMenu { get; set; }
}