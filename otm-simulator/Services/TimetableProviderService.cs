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
        private readonly ITimetableAdapter<List<Path>> _pathsDataAdapter;
        public Timetable Timetable { get; set; }

        public TimetableProviderService(IHttpClientFactory clientFactory, IOptions<AppSettings> appSettings, ITimetableAdapter<List<Path>> pathsDataAdapter)
        {
            _clientFactory = clientFactory;
            _appSettings = appSettings;
            _pathsDataAdapter = pathsDataAdapter;
            Timetable = new Timetable();
        }

        /// <summary>
        /// Main service runner. Attempts to fetch and return data from external source.
        /// </summary>
        public async Task FetchAsync()
        {
            var request = PrepareRequest(HttpMethod.Get, _appSettings.Value.OtmApiConnection.Routes.Timetable);
            var response = await GetHttpResponse(request);
            if (response.IsSuccessStatusCode)
            {
                Timetable.Paths = await _pathsDataAdapter.DeserializeAsync(response);
            }
            Timetable.UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Sends a Request based on provided HttpRequestMessage.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Http Response</returns>
        public async Task<HttpResponseMessage> GetHttpResponse(HttpRequestMessage request)
        {
            HttpClient client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_appSettings.Value.OtmApiConnection.BasePath);
            var response = await client.SendAsync(request);
            return response;
        }

        /// <summary>
        /// Prepares request based on provided method and path.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns>HttpRequestMessage object</returns>
        private HttpRequestMessage PrepareRequest(HttpMethod method, string path)
        {
            var request = new HttpRequestMessage(method, path);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "HttpClientFactory-Sample");
            return request;
        }

    }
}
