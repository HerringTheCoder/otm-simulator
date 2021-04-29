using System.Text.Json.Serialization;

namespace otm_simulator.Models
{
    public class Course
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("path_id")]
        public int PathId { get; set; }

        [JsonPropertyName("start_time")]
        public string StartTime { internal get; set; }
    }
}
