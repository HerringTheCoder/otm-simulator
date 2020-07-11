using Microsoft.AspNetCore.Mvc;
using otm_simulator.Interfaces;
using otm_simulator.Models;
using System.Collections.Generic;

namespace otm_simulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        private readonly IStateGenerator _stateGenerator;
   

        public StatesController(IStateGenerator stateGenerator)
        {
            _stateGenerator = stateGenerator;
        }

        // GET: /<StateController>
        [HttpGet]
        public List<BusState> Get()
        {
           return _stateGenerator.GetStates();
        }

        // GET /<StateController>/5
        [HttpGet("{id}")]
        public List<BusState> Get(int id)
        {
            return _stateGenerator.GetPathState(id);
        }

    }
}
