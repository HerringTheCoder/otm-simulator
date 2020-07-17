using otm_simulator.Interfaces;
using System;

namespace otm_simulator.Services
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime Now { get { return DateTime.Now; } set { } }
    }
}
