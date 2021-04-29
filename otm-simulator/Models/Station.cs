using System.Text.Json.Serialization;

namespace otm_simulator.Models
{
    public class Station
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("position")]
        public Position Position { get; set; }

        [JsonPropertyName("travel_time")]
        public int TravelTime { get; set; }
    }
}
