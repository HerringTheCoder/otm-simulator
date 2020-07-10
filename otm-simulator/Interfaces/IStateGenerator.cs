namespace otm_simulator.Interfaces
{
    public interface IStateGenerator
    {
        public void UpdateStates();

        public void CreateStatesAsync();

        public void ReleaseStates();

        public void SyncDataWithProvider();
    }
}
