using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace otm_simulator.Models
{
    public class Path
    {
        [JsonPropertyName("tid")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public int BuslineID { get; set; }

        [JsonPropertyName("stations")]
        public List<Station> Stations { get; set; }

        [JsonPropertyName("courses")]
        public List<Course> Courses { get; set; }
    }
}
