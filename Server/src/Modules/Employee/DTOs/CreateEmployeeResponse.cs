using Newtonsoft.Json;
using Shared.Result;

namespace EmployeeModule.DTOs
{
    public class CreateEmployeeResponse
    {
        [JsonProperty("eid")]
        public int EmployeeId { get; set; }

    }
}
