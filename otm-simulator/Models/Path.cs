using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace otm_simulator.Models
{
    public class Path
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("busline_id")]
        public int BuslineId { get; set; }

        [JsonPropertyName("stations")]
        public List<Station> Stations { get; set; }

        [JsonPropertyName("courses")]
        public List<Course> Courses { get; set; }
    }
}
