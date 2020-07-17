using otm_simulator.Interfaces;
using System;

namespace otm_simulator_testing.Mocks
{
    class TimeProviderMock : ITimeProvider
    {
        public DateTime Now { get; set; }
    }
}
