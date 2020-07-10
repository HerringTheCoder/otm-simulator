using System;
using System.Collections.Generic;

namespace otm_simulator.Models
{
    public class Timetable
    {
        public List<Course> Courses { get; set; }

        public List<Station> Stations { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}
