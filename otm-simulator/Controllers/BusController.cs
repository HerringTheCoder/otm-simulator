using Microsoft.AspNetCore.Mvc;

namespace otm_simulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {      
        // GET: /<BusController>
        [HttpGet]
        public string Get()
        {
            return "get";
        }

        // GET /<BusController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

    }
}
