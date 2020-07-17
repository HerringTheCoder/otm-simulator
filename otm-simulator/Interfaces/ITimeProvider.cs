using System;

namespace otm_simulator.Interfaces
{
    public interface ITimeProvider
    {
        DateTime Now { get; set; }
    }
}
