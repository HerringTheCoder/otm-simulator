namespace otm_simulator.Helpers
{
    public class AppSettings
    {
        public OtmApiConnection OtmApiConnection { get; set; }

        public int UpdateInterval { get; set; }
    }

    public class OtmApiConnection
    {
        public string BasePath { get; set; }

        public Routes Routes { get; set; }
    }

    public class Routes
    {
        public string Timetable { get; set; }
    }
}
