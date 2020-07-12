using Microsoft.Extensions.Hosting;
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

        public BackgroundProvider(ITimetableProvider timetableProvider, IStateGenerator stateGenerator)
        {
            _timetableProvider = timetableProvider;
            _stateGenerator = stateGenerator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("BackgroundProvider is attempting Courses fetch...");
                await _timetableProvider.FetchAsync();
                Console.WriteLine("Data fetch executed successfully at: " + _timetableProvider.Timetable.UpdatedAt);
                Console.WriteLine("Synchronizing StateGenerator data...");
                _stateGenerator.SyncDataWithProvider();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                Console.WriteLine();
            }
            Console.WriteLine("BackgroundProvider background task is stopping.");
        }
    }
}

