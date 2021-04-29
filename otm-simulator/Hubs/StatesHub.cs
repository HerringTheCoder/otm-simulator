using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using otm_simulator.Interfaces;
using otm_simulator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace otm_simulator.Hubs
{
    public class StatesHub : Hub<IStatesClient>
    {
        private readonly ILogger _logger;

        public StatesHub(ILogger<StatesHub> logger)
        {
            _logger = logger;
        }

        public Task ForceSendStates(List<BusState> busStates)
        {
            _logger.LogInformation("Sending states to clients...");
            return Clients.All.SendStates(busStates);
        }
    }
}
