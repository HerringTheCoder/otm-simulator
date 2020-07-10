using Microsoft.Extensions.Options;
using otm_simulator.Helpers;
using otm_simulator.Interfaces;
using otm_simulator.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace otm_simulator.Services
{
    /// <summary>
    /// Service used for fetching Timetable data from external sources (e.g. API)
    /// </summary>
    public class TimetableProviderService : ITimetableProvider
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ITimetableAdapter<List<Course>> _coursesDataAdapter;
        private readonly ITimetableAdapter<List<Station>> _stationsDataAdapter;
        public Timetable Timetable { get; set; }

        public TimetableProviderService(IHttpClientFactory clientFactory, IOptions<AppSettings> appSettings, ITimetableAdapter<List<Course>> coursesDataAdapter, ITimetableAdapter<List<Station>> stationsDataAdapter)
        {
            _clientFactory = clientFactory;
            _appSettings = appSettings;
            _coursesDataAdapter = coursesDataAdapter;
            _stationsDataAdapter = stationsDataAdapter;
            Timetable = new Timetable();
        }

        /// <summary>
        /// Main service runner. Attempts to fetch and return data from external source.
        /// </summary>
        public async Task FetchCoursesAsync()
        {
            var request = PrepareRequest(HttpMethod.Get, _appSettings.Value.OtmApiConnection.Routes.Courses);
            var response = await FetchData(request);
            if (response.IsSuccessStatusCode)
            {
                Timetable.Courses = await _coursesDataAdapter.DeserializeAsync(response);
            }
         
            Timetable.UpdatedAt = DateTime.Now;
        }

        public async Task FetchStationsAsync(int pathID)
        {

            var request = PrepareRequest(HttpMethod.Get, _appSettings.Value.OtmApiConnection.Routes.Stations);
            var response = await FetchData(request);
            if (response.IsSuccessStatusCode)
            {
                Timetable.Stations = await _stationsDataAdapter.DeserializeAsync(response);
            }
        }
        public async Task<HttpResponseMessage> FetchData(HttpRequestMessage request)
        {
            HttpClient client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_appSettings.Value.OtmApiConnection.BasePath);                        
            var response = await client.SendAsync(request);          
            return response;
        }

        private HttpRequestMessage PrepareRequest(HttpMethod method, string path)
        {
            var request = new HttpRequestMessage(method, path);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "HttpClientFactory-Sample");
            return request;
        }

    }
}
