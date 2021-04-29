using System.Collections.Generic;
using System.Threading.Tasks;
using otm_simulator.Models;

namespace otm_simulator.Hubs
{
    public interface IStatesClient
    {
        Task SendStates(List<BusState> busStates);
    }
}
