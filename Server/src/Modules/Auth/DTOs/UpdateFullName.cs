using Newtonsoft.Json;

namespace AuthModule.DTOs;

public class UpdateFullNameSigupResponse
{
    [JsonProperty("refresh_token", NullValueHandling = NullValueHandling.Ignore)]
    public string? RefreshToken { get; set; }

    [JsonProperty("access_token", NullValueHandling = NullValueHandling.Ignore)]
    public string? AccessToken { get; set; }
}

public class UpdateFullNameResquest
{
    public string? Phone { get; set; }
    public string? Mail { get; set; }
    public string FullName { get; set; }
}

