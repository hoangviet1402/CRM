using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AuthModule.DTOs;

public class ChangePassRequest
{
    [Required]
    [JsonProperty("shop_id")]
    public int ShopId { get; set; }

    [Required]
    [JsonProperty("user_id")]
    public int UserId { get; set; }

    [Required]
    [JsonProperty("new_pass")]
    public string NewPass { get; set; }
    

    [JsonProperty("old_pass")]
    public string OldPass { get; set; }
} 