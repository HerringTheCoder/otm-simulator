using Microsoft.VisualBasic.CompilerServices;
using System.Text.Json.Serialization;

namespace otm_simulator.Models
{
    public class Position
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }
}
