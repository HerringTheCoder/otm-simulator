using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using otm_simulator.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace otm_simulator.Services
{
    public class BackgroundProvider : BackgroundService
    {
        private readonly ITimetableProvider _timetableProvider;
        private readonly IStateGenerator _stateGenerator;
        private readonly ILogger _logger;
        public BackgroundProvider(ITimetableProvider timetableProvider, 
            IStateGenerator stateGenerator,
            ILogger<BackgroundProvider> logger
            )
        {
            _timetableProvider = timetableProvider;
            _stateGenerator = stateGenerator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("BackgroundProvider is attempting Courses fetch...");
                    await _timetableProvider.FetchAsync();
                    _logger.LogInformation("Data fetch executed successfully at: " +
                                           _timetableProvider.Timetable.UpdatedAt);
                    _logger.LogInformation("Synchronizing StateGenerator data...");
                    _stateGenerator.SyncDataWithProvider();
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw;
                }
            }
            _logger.LogInformation("BackgroundProvider background task is stopping.");
        }
    }
}

