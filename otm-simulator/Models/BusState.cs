using otm_simulator.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace otm_simulator.Models
{    
    public class BusState
    {
        public Position CurrentPosition { get; set; }

        public Status Status { get; set; }

        public int Delay { get; set; }

        public Course Course { get; set; }

        public List<Station> Stations { internal get; set; }

        public int DestinationStationIndex { internal get; set; }

        public int ExecutedSteps { internal get; set; }

        public int EstimatedSteps { internal get; set; }

        public BusState(List<Station> stations, Course course, int UpdateInterval)
        {
            CurrentPosition = stations.First().Position;
            Status = Status.Driving;
            Delay = 0;
            Course = course;
            Stations = stations;
            DestinationStationIndex = 1;
            ExecutedSteps = 0;
            EstimatedSteps = CalculateEstimatedSteps(UpdateInterval);
            while (EstimatedSteps == 0 && DestinationStationIndex <= Stations.Count())
            {
                SetNextDestination();
                EstimatedSteps = CalculateEstimatedSteps(UpdateInterval);
            }
        }

        /// <summary>
        /// Calculates estimated travel time between two stations
        /// </summary>
        private int CalculateEstimatedSteps(int updateInterval)
        {
            int travelTime1 = Stations[DestinationStationIndex - 1].TravelTime;
            int travelTime2 = Stations[DestinationStationIndex].TravelTime;
            int estimatedSteps = (travelTime2 - travelTime1) * 60 / updateInterval;
            return estimatedSteps;
        }

        /// <summary>
        /// Calculates updated position
        /// </summary>
        public void CalculateNextStepPosition()
        {
            Position start = Stations[DestinationStationIndex - 1].Position;
            Position finish = Stations[DestinationStationIndex].Position;
            ExecutedSteps++;
            CurrentPosition = new Position
            {
                Lat = start.Lat + (finish.Lat - start.Lat) * ((double)ExecutedSteps / EstimatedSteps),
                Lng = start.Lng + (finish.Lng - start.Lng) * ((double)ExecutedSteps / EstimatedSteps),
            };
        }

        /// <summary>
        /// Sets parameters for next destination
        /// </summary>
        public void SetNextDestination()
        {
            DestinationStationIndex++;
            ExecutedSteps = 0;
            CurrentPosition = Stations[DestinationStationIndex - 1].Position;
        }
    }
}
