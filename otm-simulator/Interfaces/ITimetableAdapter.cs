using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace otm_simulator.Interfaces
{
    public interface ITimetableAdapter<T>
    {
        public Task<T> DeserializeAsync(HttpResponseMessage response);

        public Task<Stream> SerializeAsync(T obj);
    }
}
