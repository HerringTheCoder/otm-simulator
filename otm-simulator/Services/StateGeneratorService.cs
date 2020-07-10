using Microsoft.Extensions.Options;
using otm_simulator.Helpers;
using otm_simulator.Interfaces;
using otm_simulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace otm_simulator.Services
{
    public class StateGeneratorService : IStateGenerator
    {
        private static readonly string[] Statuses = new[]
         {
            "Driving", "Standing by..."
        };
        public List<BusState> BusStates { get; set; }

        private readonly ITimetableProvider _timetableProvider;
        private IEnumerable<Course> courses;
        private IEnumerable<Station> stations;
        private IOptions<AppSettings> _appSettings;

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
            courses = _timetableProvider.Timetable.Courses;
            stations = _timetableProvider.Timetable.Stations;
        }

        /// <summary>
        /// Updates all available states by drawing random Status and updating CurrentPosition value
        /// </summary>
        public void UpdateStates()
        {
            var random = new Random();
            foreach (BusState busState in BusStates)
            {
                busState.Status = Statuses[random.Next(2)];
                if (busState.Status == "Driving")
                {
                    busState.CalculateNextStepPosition();

                    Console.WriteLine("BusState position changed to Y:{0}, X:{1}",
                        busState.CurrentPosition.Lat,
                        busState.CurrentPosition.Lng);
                }
                else
                {
                    Console.WriteLine("BusState position unchanged...");
                }
            }
            Console.WriteLine("Updated {0} BusState(s)", BusStates.Count());
        }

        /// <summary>
        /// Releases excessively delayed BusStates
        /// </summary>
        public void ReleaseStates()
        {
            int removedItemsCount = BusStates.RemoveAll(item => item.Delay.TotalMinutes >= 30);
            if (removedItemsCount > 0)
            {
                Console.WriteLine("Released {0} BusStates due to the excessive delay.", removedItemsCount);
            }
        }

        /// <summary>
        /// Creates new BusStates based on expected schedule
        /// </summary>
        public async Task CreateStatesAsync()
        {
            DateTime currentTime = DateTime.Now;
            foreach (Course course in courses)
            {
                var startTime = DateTime.Parse(course.StartTime);
                if (currentTime > startTime
                    && currentTime <= startTime.AddMinutes(stations.Last().TravelTime)
                    && !BusStates.Any(item => item.Course.ID == course.ID))
                {
                    await _timetableProvider.FetchStationsAsync(course.PathID);
                    BusStates.Add(new BusState((List<Station>)stations, course, _appSettings.Value.UpdateInterval));
                    Console.WriteLine("Successfully created a new BusState");
                }
            }
        }
    }
}
