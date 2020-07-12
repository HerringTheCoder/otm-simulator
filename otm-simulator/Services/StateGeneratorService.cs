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
        private readonly ITimetableProvider _timetableProvider;
        private IEnumerable<Path> paths;
        private readonly IOptions<AppSettings> _appSettings;

        public StateGeneratorService(ITimetableProvider timetableProvider, IOptions<AppSettings> appSettings)
        {
            _timetableProvider = timetableProvider;
            _appSettings = appSettings;
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
                busState.Status = GetRandomizedStatus();
                if (busState.Status == "Driving")
                {
                    busState.CalculateNextStepPosition();

                    Console.WriteLine("BusState position changed to Y:{0}, X:{1}, Current Progress: {2}/{3}, Overall Progress: {4}/{5}, Delay: {6}",
                        busState.CurrentPosition.Lat,
                        busState.CurrentPosition.Lng,
                        busState.ExecutedSteps,
                        busState.EstimatedSteps,
                        busState.DestinationStationIndex,
                        busState.Stations.Count(),
                        busState.Delay);
                }
                else if (busState.Status == "Delayed")
                {
                    Console.WriteLine("BusState delay has increased.");
                    busState.Delay += _appSettings.Value.UpdateInterval;
                }
                else if (busState.Status == "Standing by")
                {
                    Console.WriteLine("BusState position unchanged");
                    busState.ExecutedSteps++;
                }
                else
                {
                    Console.WriteLine("Unrecognized status!");
                }

                while (busState.ExecutedSteps >= busState.EstimatedSteps)
                {
                    Console.WriteLine("Next station reached!");
                    busState.SetNextDestination();
                }
            }
            Console.WriteLine("Updated {0} BusState(s)", BusStates.Count());
        }

        /// <summary>
        /// Releases overdue (delayed or finished) BusStates
        /// </summary>
        public void ReleaseStates()
        {
            int removedItemsCount = BusStates.RemoveAll(item =>
        DateTime.Now.TimeOfDay > DateTime.Parse(item.Course.StartTime).AddMinutes(item.Stations.Last().TravelTime).AddSeconds(item.Delay).TimeOfDay ||
        item.Delay >= 15 * 60 ||
        item.DestinationStationIndex == item.Stations.Count);
            if (removedItemsCount > 0)
            {
                Console.WriteLine("Released {0} overdue states", removedItemsCount);
            }
        }

        /// <summary>
        /// Creates new BusStates based on expected schedule
        /// </summary>
        public void CreateStates()
        {
            DateTime currentTime = DateTime.Now;
            foreach (Path path in paths)
            {
                foreach (Course course in path.Courses)
                {
                    var startTime = DateTime.Parse(course.StartTime);
                    if (currentTime < startTime.AddSeconds(_appSettings.Value.UpdateInterval) &&
                        currentTime > startTime.AddSeconds(-_appSettings.Value.UpdateInterval) &&
                        !BusStates.Any(item => item.Course.ID == course.ID))
                    {
                        BusStates.Add(new BusState(path.Stations, course, _appSettings.Value.UpdateInterval));
                        Console.WriteLine("Successfully created a new BusState");
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
                if ((statusRatio.Value - rngNumber) > 0)
                { return statusRatio.Key; }
            }
            return "Unknown";
        }
    }
}
