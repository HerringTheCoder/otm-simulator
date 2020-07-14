using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using otm_simulator.Interfaces;
using otm_simulator.Models;
using System.Threading.Tasks;

namespace otm_simulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateController : ControllerBase
    {
        private readonly ITimetableProvider _timetableProvider;
        private readonly IStateGenerator _stateGenerator;
        private readonly ILogger<UpdateController> _logger;

        public UpdateController(ILogger<UpdateController> logger,
            ITimetableProvider timetableProvider,
            IStateGenerator stateGenerator)
        {
            _logger = logger;
            _timetableProvider = timetableProvider;
            _stateGenerator = stateGenerator;
        }

        [HttpGet]
        public async Task<Timetable> Get()
        {
            _logger.LogInformation("Forced update requested");
            await _timetableProvider.FetchAsync();
            _stateGenerator.SyncDataWithProvider();
            return _timetableProvider.Timetable;
        }
    }
}
