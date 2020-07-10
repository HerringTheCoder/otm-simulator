using System;
using System.Collections.Generic;

namespace otm_simulator.Models
{
    public class Timetable
    {
        public List<Path> Paths { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
