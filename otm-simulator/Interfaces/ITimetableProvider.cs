using otm_simulator.Models;
using System.Threading.Tasks;

namespace otm_simulator.Interfaces
{
    public interface ITimetableProvider
    {
        public Timetable Timetable { get; set; }

        public Task FetchCoursesAsync();

        public Task FetchStationsAsync(int pathID);
    }
}
