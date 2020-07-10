using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace otm_simulator.Interfaces
{
    public interface IStateGenerator
    {
        public void UpdateStates();

        public Task CreateStatesAsync();

        public void ReleaseStates();

        public void SyncDataWithProvider();
    }
}
