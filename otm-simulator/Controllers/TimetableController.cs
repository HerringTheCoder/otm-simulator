using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using otm_simulator.Interfaces;
using otm_simulator.Models;

namespace otm_simulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimetableController : ControllerBase
    {       
        private readonly ITimetableProvider _timetableProvider;
        private readonly ILogger<TimetableController> _logger;

        public TimetableController(ILogger<TimetableController> logger, ITimetableProvider timetableProvider)
        {
            _logger = logger;
            _timetableProvider = timetableProvider;
        }

        [HttpGet]
        public async Task<Timetable> Get(int id)
        {
            _logger.LogInformation("Get timetable route accessed");
            await _timetableProvider.FetchCoursesAsync();
            return _timetableProvider.Timetable;
        }
    }
}
