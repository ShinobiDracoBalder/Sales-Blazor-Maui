using Newtonsoft.Json;

namespace SBM.Shared.Responses
{
    public class CityResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; } = null;   
    }
}
