using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using otm_simulator.Helpers;
using otm_simulator.Hubs;
using otm_simulator.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace otm_simulator.Services
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly ITimetableProvider _timetableProvider;
        private readonly IStateGenerator _stateGenerator;
        private readonly IOptions<AppSettings> _appSettings;
        //private readonly IHubContext<StatesHub> _hubContext;
        private readonly ILogger _logger;

        public BackgroundWorker(
            ITimetableProvider timetableProvider,
            IStateGenerator stateGenerator,
            IOptions<AppSettings> appSettings,
            //IHubContext<StatesHub> hubContext,
            ILogger<BackgroundWorker> logger)
        {
            _timetableProvider = timetableProvider;
            _stateGenerator = stateGenerator;
            _appSettings = appSettings;
            //_hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_timetableProvider.Timetable.UpdatedAt != null)
                {
                    _logger.LogInformation("BackgroundWorker is releasing overdue states... ");
                    _stateGenerator.ReleaseStates();
                    _logger.LogInformation("BackgroundWorker is detecting and creating states...");
                    _stateGenerator.CreateStates();
                    _logger.LogInformation("BackgroundWorker is updating states...");
                    _stateGenerator.UpdateStates();
                    //await _hubContext.Clients.All.SendAsync("SendStates", _stateGenerator.GetStates());
                }
                else
                {
                    _logger.LogInformation("BackgroundWorker is missing important Timetable data");
                }
                await Task.Delay(TimeSpan.FromSeconds(_appSettings.Value.UpdateInterval), stoppingToken);
            }
            _logger.LogInformation("BackgroundWorker background task is stopping.");
        }
    }
}
