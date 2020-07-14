using otm_simulator.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace otm_simulator.Services
{
    public class TimetableAdapterService<T> : ITimetableAdapter<T>
    {
        /// <summary>
        /// Deserializes JSON response into provided class object
        /// </summary>
        /// <param name="response"></param>
        async Task<T> ITimetableAdapter<T>.DeserializeAsync(HttpResponseMessage response)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(responseStream);
        }

        /// <summary>
        /// Serializes provided object into JSON response 
        /// </summary>
        /// <param name="obj"></param>
        string ITimetableAdapter<T>.Serialize(T obj)
        {
            return JsonSerializer.Serialize<T>(obj);
        }
    }
}
