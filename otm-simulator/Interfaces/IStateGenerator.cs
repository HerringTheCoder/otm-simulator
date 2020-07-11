using otm_simulator.Models;
using System.Collections.Generic;

namespace otm_simulator.Interfaces
{
    public interface IStateGenerator
    {
        public void UpdateStates();

        public void CreateStates();

        public void ReleaseStates();

        public void SyncDataWithProvider();

        public List<BusState> GetStates();

        public List<BusState> GetPathState(int id);
    }
}
