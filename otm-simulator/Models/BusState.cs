using System;
using System.Collections.Generic;
using System.Linq;

namespace otm_simulator.Models
{
    public class BusState
    {
        public Position CurrentPosition { get; private set; }

        public string Status { get; private set; }

        public int Delay { get; private set; }

        public Course Course { get; private set; }

        internal List<Station> Stations { get; private set; }

        internal int DestinationStationIndex { get; private set; }

        public Dictionary<string, Func<string>> ActionDictionary { get; set; }

        private int _executedSteps;
        private int _estimatedSteps;      

        public BusState(List<Station> stations, Course course, int updateInterval)
        {
            CurrentPosition = stations.First().Position;
            Status = "Standing";
            Delay = 0;
            Course = course;
            Stations = stations;
            DestinationStationIndex = 1;
            _executedSteps = 0;
            CalculateEstimatedSteps(updateInterval);
            while (CheckIfStationIsReached())
            {
                SetNextDestination();
                CalculateEstimatedSteps(updateInterval);
            }
            ActionDictionary = new Dictionary<string, Func<string>>
            {
                { "Driving", new Func<string>(() => {
                    CalculateNextStepPosition();
                    Status = "Driving";
                    return $"BusState position changed. Current Progress: {_executedSteps}/{_estimatedSteps}, Overall Progress: {DestinationStationIndex}/{Stations.Count()}, Delay: {Delay}";
                })
                },

                { "Standing", new Func<string>(() => {
                    _executedSteps++;
                    Status = "Standing";
                    return "BusState position unchanged";
                })
                },

                { "Delayed", new Func<string>(() => {
                    Delay++;
                    Status = "Delayed";
                    return "BusState delay has increased.";
                })
                }
            };
        }

        /// <summary>
        /// Calculates estimated travel time between two stations
        /// </summary>
        public void CalculateEstimatedSteps(int updateInterval)
        {
            int travelTime1 = Stations[DestinationStationIndex - 1].TravelTime;
            int travelTime2 = Stations[DestinationStationIndex].TravelTime;
            _estimatedSteps = (travelTime2 - travelTime1) * 60 / updateInterval;
        }

        /// <summary>
        /// Calculates updated position
        /// </summary>
        public void CalculateNextStepPosition()
        {
            Position start = Stations[DestinationStationIndex - 1].Position;
            Position finish = Stations[DestinationStationIndex].Position;
            _executedSteps++;
            CurrentPosition = new Position
            {
                Latitude = start.Latitude + (finish.Latitude - start.Latitude) * ((double)_executedSteps / _estimatedSteps),
                Longitude = start.Longitude + (finish.Longitude - start.Longitude) * ((double)_executedSteps / _estimatedSteps),
            };
        }

        /// <summary>
        /// Sets parameters for next destination
        /// </summary>
        public void SetNextDestination()
        {
            DestinationStationIndex++;
            _executedSteps = 0;
            CurrentPosition = Stations[DestinationStationIndex - 1].Position;
        }

        public bool CheckIfStationIsReached()
        {
            return _executedSteps >= _estimatedSteps;   
        }

        public bool CheckIfCourseIsFinished()
        {
            return DestinationStationIndex == Stations.Count;
        }
    }
}
