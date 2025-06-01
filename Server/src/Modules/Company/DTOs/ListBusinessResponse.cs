using Newtonsoft.Json;

namespace Company.DTOs
{
    public class ListBusinessResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("index_num")]
        public int IndexNum { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }
    }
}
