using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using otm_simulator.Helpers;
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

        public BackgroundWorker(
            ITimetableProvider timetableProvider,
            IStateGenerator stateGenerator,
            IOptions<AppSettings> appSettings)
        {
            _timetableProvider = timetableProvider;
            _stateGenerator = stateGenerator;
            _appSettings = appSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_timetableProvider.Timetable.UpdatedAt != null)
                {
                    Console.WriteLine("BackgroundWorker is releasing overdue states... ");
                    _stateGenerator.ReleaseStates();
                    Console.WriteLine("BackgroundWorker is detecting and creating states...");
                    _stateGenerator.CreateStatesAsync();
                    Console.WriteLine("BackgroundWorker is updating states...");
                    _stateGenerator.UpdateStates();
                }
                else
                {
                    Console.WriteLine("BackgroundWorker is missing important Timetable data");
                }
                await Task.Delay(TimeSpan.FromSeconds(_appSettings.Value.UpdateInterval), stoppingToken);
            }
            Console.WriteLine("BackgroundWorker background task is stopping.");
        }
    }
}
