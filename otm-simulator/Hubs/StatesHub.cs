using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using otm_simulator.Interfaces;
using otm_simulator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace otm_simulator.Hubs
{
    public class StatesHub : Hub
    {
        private readonly ITimetableAdapter<List<BusState>> _busStatesAdapter;
        private readonly ILogger _logger;

        public StatesHub(ITimetableAdapter<List<BusState>> busStatesAdapter, ILogger<StatesHub> logger)
        {
            _busStatesAdapter = busStatesAdapter;
            _logger = logger;
        }

        public Task SendStates(List<BusState> busStates)
        {
            _logger.LogInformation("Sending states to clients...");
            return Clients.All.SendAsync(_busStatesAdapter.Serialize(busStates));
        }
    }
}
