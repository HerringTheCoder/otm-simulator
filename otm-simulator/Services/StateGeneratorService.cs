using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using otm_simulator.Helpers;
using otm_simulator.Interfaces;
using otm_simulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace otm_simulator.Services
{
    public class StateGeneratorService : IStateGenerator
    {
        public List<BusState> BusStates { get; set; }
        private IEnumerable<Path> paths;
        private readonly ITimetableProvider _timetableProvider;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger<StateGeneratorService> _logger;
        private readonly ITimeProvider _timeProvider;

        public StateGeneratorService(ITimetableProvider timetableProvider,
            IOptions<AppSettings> appSettings,
            ILogger<StateGeneratorService> logger,
            ITimeProvider timeProvider)
        {
            _timetableProvider = timetableProvider;
            _appSettings = appSettings;
            _logger = logger;
            _timeProvider = timeProvider;
            BusStates = new List<BusState>();
        }

        /// <summary>
        /// Updates data to match the current state of TimetableProvider
        /// </summary>
        public void SyncDataWithProvider()
        {
            paths = _timetableProvider.Timetable.Paths;
        }

        /// <summary>
        /// Updates all available states by drawing random Status and updating CurrentPosition value
        /// </summary>
        public void UpdateStates()
        {
            foreach (BusState busState in BusStates)
            {
                Enum.TryParse(GetRandomizedStatus(), out Status result);
                busState.Status = result;
                Action action = busState.Status switch
                {
                    Status.Driving => () =>
                    {
                        busState.CalculateNextStepPosition();

                        _logger.LogInformation("BusState position changed to Y:{0}, X:{1}, Current Progress: {2}/{3}, Overall Progress: {4}/{5}, Delay: {6}",
                            busState.CurrentPosition.Lat,
                            busState.CurrentPosition.Lng,
                            busState.ExecutedSteps,
                            busState.EstimatedSteps,
                            busState.DestinationStationIndex,
                            busState.Stations.Count(),
                            busState.Delay);
                    },                    
                    Status.Delayed => () =>
                    {

                        _logger.LogInformation("BusState delay has increased.");
                        busState.Delay += _appSettings.Value.UpdateInterval;
                    },                    
                    Status.Standing => () =>
                    {
                        _logger.LogInformation("BusState position unchanged");
                        busState.ExecutedSteps++;
                    },
                    Status.Unknown => () =>
                    {
                        _logger.LogInformation("Unrecognized status!");
                    },
                    _ => throw new NotImplementedException()
                };
                action();
                while (busState.ExecutedSteps >= busState.EstimatedSteps)
                {
                    _logger.LogInformation("Next station reached!");
                    busState.SetNextDestination();
                }
            }
            _logger.LogInformation("Updated {0} BusState(s)", BusStates.Count());
        }

        /// <summary>
        /// Releases overdue (delayed or finished) BusStates
        /// </summary>
        public void ReleaseStates()
        {
            int removedItemsCount = BusStates.RemoveAll(item =>
        _timeProvider.Now.TimeOfDay > DateTime.Parse(item.Course.StartTime).AddMinutes(item.Stations.Last().TravelTime).AddSeconds(item.Delay).TimeOfDay ||
        item.Delay >= 15 * 60 ||
        item.DestinationStationIndex == item.Stations.Count);
            if (removedItemsCount > 0)
            {
                _logger.LogInformation("Released {0} overdue states", removedItemsCount);
            }
        }

        /// <summary>
        /// Creates new BusStates based on expected schedule
        /// </summary>
        public void CreateStates()
        {
            foreach (Path path in paths)
            {
                foreach (Course course in path.Courses)
                {
                    var startTime = DateTime.Parse(course.StartTime);
                    if (_timeProvider.Now < startTime.AddSeconds(_appSettings.Value.UpdateInterval) &&
                        _timeProvider.Now > startTime.AddSeconds(-_appSettings.Value.UpdateInterval) &&
                        !BusStates.Any(item => item.Course.ID == course.ID))
                    {
                        BusStates.Add(new BusState(path.Stations, course, _appSettings.Value.UpdateInterval));
                        _logger.LogInformation("Successfully created a new BusState");
                    }
                }
            }
        }

        /// <summary>
        /// Get list of all active BusState objects
        /// </summary>
        /// <returns>List of BusState objects</returns>
        public List<BusState> GetStates()
        {
            return BusStates;
        }

        /// <summary>
        /// Get list of all active BusState objects which contain provided PathID
        /// </summary>
        /// <param name="pathId"></param>
        /// <returns>List of BusState objects</returns>
        public List<BusState> GetPathState(int pathId)
        {
            return BusStates
                .Where(busState => busState.Course.PathID == pathId)
                .ToList();
        }

        /// <summary>
        /// Gets randomized status based on pre-configured weights
        /// </summary>
        /// <returns>string containing drawn status name</returns>
        private string GetRandomizedStatus()
        {
            Dictionary<string, double> statusRatioDictionary = _appSettings.Value.StatusRatio;
            var random = new Random();
            double totalRatio = statusRatioDictionary.Sum(statusRatio => statusRatio.Value);
            double rngNumber = random.NextDouble() * totalRatio;
            foreach (var statusRatio in statusRatioDictionary.OrderBy(item => item.Value))
            {
                if ((statusRatio.Value*totalRatio - rngNumber) < 0)
                { return statusRatio.Key; }
            }
            return "Unknown";
        }
    }
}
