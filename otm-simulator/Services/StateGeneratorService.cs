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
        private IEnumerable<Path> _paths;
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
            _paths = _timetableProvider.Timetable.Paths;
        }

        /// <summary>
        /// Updates all available states by drawing random Status and updating CurrentPosition value
        /// </summary>
        public void UpdateStates()
        {
            foreach (BusState busState in BusStates)
            {
                string drawnStatus = "";
                try
                {
                    drawnStatus = GetRandomizedStatus();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return;
                }           
                _logger.LogInformation(busState.ActionDictionary[drawnStatus].Invoke());
                while(busState.CheckIfStationIsReached())
                {
                    busState.SetNextDestination();
                    busState.CalculateEstimatedSteps(_appSettings.Value.UpdateIntervalInSeconds);
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
        item.CheckIfCourseIsFinished());
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
            foreach (Path path in _paths)
            {
                foreach (Course course in path.Courses)
                {
                    var startTime = DateTime.Parse(course.StartTime);
                    int updateIntervalInSeconds = _appSettings.Value.UpdateIntervalInSeconds;
                    if (_timeProvider.Now < startTime.AddSeconds(updateIntervalInSeconds) &&
                        _timeProvider.Now > startTime.AddSeconds(-updateIntervalInSeconds) && BusStates.All(item => item.Course.Id != course.Id))
                    {
                        BusStates.Add(new BusState(path.Stations, course, updateIntervalInSeconds));
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
        /// Get list of all active BusState objects which contain provided PathId
        /// </summary>
        /// <param name="pathId"></param>
        /// <returns>List of BusState objects</returns>
        public List<BusState> GetPathState(int pathId)
        {
            return BusStates
                .Where(busState => busState.Course.PathId == pathId)
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
            double rngNumber = random.NextDouble() * statusRatioDictionary.Sum(statusRatio => statusRatio.Value); ;
            statusRatioDictionary.OrderBy(item => item.Value);
            double upperBound = 0;
            foreach (KeyValuePair<string, double> statusRatio in statusRatioDictionary)
            {
                upperBound += statusRatio.Value;
                if (rngNumber <= upperBound)
                {
                    return statusRatio.Key;
                }
            }
            throw new ArgumentOutOfRangeException("Unknown status value");
        }
    }
}
