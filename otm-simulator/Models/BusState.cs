using System;
using System.Collections.Generic;
using System.Linq;

namespace otm_simulator.Models
{
    public class BusState
    {
        public Position CurrentPosition { get; set; }

        public string Status { get; set; }

        public TimeSpan Delay { get; set; }

        public Course Course { get; set; }

        public List<Station> Stations { get; set; }

        public int DestinationStationIndex { get; set; }

        public int ExecutedSteps { get; set; }

        public int EstimatedSteps { get; set; }

        public BusState(List<Station> stations, Course course, int UpdateInterval)
        {
            CurrentPosition = stations.First().Position;
            Status = "Standing by...";
            Delay = new TimeSpan(0);
            Course = course;
            Stations = stations;
            DestinationStationIndex = 0;
            ExecutedSteps = 0;
            EstimatedSteps = CalculateEstimatedSteps(UpdateInterval);
        }

        /// <summary>
        /// Calculates estimated travel time between two stations
        /// </summary>
        private int CalculateEstimatedSteps(int UpdateInterval)
        {
            int travelTime1 = Stations[DestinationStationIndex].TravelTime;
            int travelTime2 = Stations[DestinationStationIndex + 1].TravelTime;
            int estimatedSteps = (travelTime2 - travelTime1) * 60 / UpdateInterval;
            if (estimatedSteps == 0 && ExecutedSteps >= estimatedSteps)
            {
                SetNextDestination();
            }
            return estimatedSteps;
        }

        /// <summary>
        /// Calculates updated position
        /// </summary>
        public void CalculateNextStepPosition()
        {
            Position start = Stations[DestinationStationIndex].Position;
            Position finish = Stations[DestinationStationIndex + 1].Position;
            ExecutedSteps++;
            CurrentPosition = new Position
            {
                Lat = start.Lat + (finish.Lat - start.Lat) * ((double)ExecutedSteps / EstimatedSteps),
                Lng = start.Lng + (finish.Lng - start.Lng) * ((double)ExecutedSteps / EstimatedSteps),
            };
            if (CurrentPosition.Lat == finish.Lat && CurrentPosition.Lng == finish.Lat)
            {
                Console.WriteLine("STATION REACHED");
                SetNextDestination();
            }
        }

        /// <summary>
        /// Sets parameters for next destination
        /// </summary>
        public void SetNextDestination()
        {
            DestinationStationIndex++;
            EstimatedSteps = CalculateEstimatedSteps(5);
            ExecutedSteps = 0;
            CurrentPosition = Stations[DestinationStationIndex++].Position;
        }
    }
}
