using Newtonsoft.Json;

namespace Company.DTOs;


public class CreateBranchesRequest
{
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
    public string Address { get; set; }

    [JsonProperty("region_id", NullValueHandling = NullValueHandling.Ignore)]
    public string RegionId { get; set; }

    [JsonProperty("is_onboarding", NullValueHandling = NullValueHandling.Ignore)]
    public int IsOnboarding { get; set; }

    [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
    public int Latitude { get; set; }

    [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
    public int Longitude { get; set; }
} 

public class CreateBranchesResponse
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public int Id { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
    public RegionInfo Region { get; set; }

    [JsonProperty("address_lat", NullValueHandling = NullValueHandling.Ignore)]
    public double AddressLat { get; set; }

    [JsonProperty("address_lng", NullValueHandling = NullValueHandling.Ignore)]
    public double AddressLng { get; set; }

    [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
    public string Country { get; set; }

    [JsonProperty("province", NullValueHandling = NullValueHandling.Ignore)]
    public string Province { get; set; }

    [JsonProperty("district", NullValueHandling = NullValueHandling.Ignore)]
    public string District { get; set; }

    [JsonProperty("tel", NullValueHandling = NullValueHandling.Ignore)]
    public string? Tel { get; set; }

    [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
    public string Address { get; set; }

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }

    [JsonProperty("is_headquarter", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsHeadquarter { get; set; }

    [JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
    public string CreatedAt { get; set; }

    [JsonProperty("alias", NullValueHandling = NullValueHandling.Ignore)]
    public string Alias { get; set; }

    [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
    public string? Color { get; set; }

    [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
    public string Code { get; set; }

    [JsonProperty("sort_index", NullValueHandling = NullValueHandling.Ignore)]
    public int SortIndex { get; set; }

    [JsonProperty("phone_code", NullValueHandling = NullValueHandling.Ignore)]
    public int PhoneCode { get; set; }
}

public class RegionInfo
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public int Id { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }
}



// {
//   "name": "Chi nhánh 1",
//   "address": "Hà Nội, Việt Nam",
//   "region_id": "683c61d95592a0ec4f0ee6d8",
//   "is_onboarding": 1,
//   "address_lat": 21.02139,
//   "address_lng": 105.8523
// }