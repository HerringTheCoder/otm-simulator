using System.Collections.Generic;

namespace otm_simulator.Helpers
{
    public class AppSettings
    {
        public StatusRatio StatusRatio { get; set; }

        public OtmApiConnection OtmApiConnection { get; set; }

        public int UpdateIntervalInSeconds { get; set; }
    }

    public class StatusRatio : Dictionary<string, double> { }

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
