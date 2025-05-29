using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AuthModule.DTOs;

public class CreatePassRequest
{
    [Required]
    [JsonProperty("shop_id")]
    public int ShopId { get; set; }

    [Required]
    [JsonProperty("user_id")]
    public int UserId { get; set; }

    [Required]
    [JsonProperty("password")]
    public string Password { get; set; }
    

    [JsonProperty("comfirmPass")]
    public string ComfirmPass { get; set; }
}


