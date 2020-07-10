using Microsoft.AspNetCore.Mvc;

namespace otm_simulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PathController : ControllerBase
    {
        // GET: /<PathController>
        [HttpGet]
        public string Get()
        {
            return "get";
        }

        // GET /<PathController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

    }
}
