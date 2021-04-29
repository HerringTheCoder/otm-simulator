using otm_simulator.Interfaces;
using otm_simulator.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace otm_simulator.Tests
{
    public class TimetableProviderMock : ITimetableProvider
    {
        public Timetable Timetable { get; set; }

        public TimetableProviderMock()
        {
            Timetable = new Timetable
            {
                Paths = new List<Path>()
            };
            for (int p = 1; p <= 5; p++)
            {
                Timetable.Paths.Add(new Path() { Id = p, BuslineId = p, Stations = new List<Station>(), Courses = new List<Course>() });
            }
            foreach (Path path in Timetable.Paths)
            {
                var random = new Random();
                path.Courses.Add(new Course() { Id = random.Next(), PathId = path.Id, StartTime = DateTime.Now.AddSeconds(0).TimeOfDay.ToString() });
                path.Courses.Add(new Course() { Id = random.Next(), PathId = path.Id, StartTime = DateTime.Now.AddSeconds(1).TimeOfDay.ToString() });
                path.Courses.Add(new Course() { Id = random.Next(), PathId = path.Id, StartTime = DateTime.Now.AddSeconds(2).TimeOfDay.ToString() });
                for (int s = 1; s <= 5; s++)
                {
                    path.Stations.Add(new Station() { Id = s, Name = "", Position = new Position() { Latitude = s + 50, Longitude = s + 51 }, TravelTime = s });
                }
            }

        }

        public Task FetchAsync()
        {
            Timetable.UpdatedAt = DateTime.Now;
            return Task.CompletedTask;
        }
    }
}


