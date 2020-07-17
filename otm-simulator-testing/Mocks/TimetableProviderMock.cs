﻿using otm_simulator.Interfaces;
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
                Timetable.Paths.Add(new Path() { ID = p, BuslineID = p, Stations = new List<Station>(), Courses = new List<Course>() });
            }
            foreach (Path path in Timetable.Paths)
            {
                var random = new Random();
                path.Courses.Add(new Course() { ID = random.Next(), PathID = path.ID, StartTime = DateTime.Now.AddSeconds(0).TimeOfDay.ToString() });
                path.Courses.Add(new Course() { ID = random.Next(), PathID = path.ID, StartTime = DateTime.Now.AddSeconds(1).TimeOfDay.ToString() });
                path.Courses.Add(new Course() { ID = random.Next(), PathID = path.ID, StartTime = DateTime.Now.AddSeconds(2).TimeOfDay.ToString() });
                for (int s = 1; s <= 5; s++)
                {
                    path.Stations.Add(new Station() { ID = s, Name = "", Position = new Position() { Lat = s + 50, Lng = s + 51 }, TravelTime = s });
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


