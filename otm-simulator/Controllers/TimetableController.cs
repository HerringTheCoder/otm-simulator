﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using otm_simulator.Interfaces;
using otm_simulator.Models;
using System.Threading.Tasks;

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
        public async Task<Timetable> Get()
        {
            _logger.LogInformation("Get /timetable route accessed");
            await _timetableProvider.FetchAsync();
            return _timetableProvider.Timetable;
        }
    }
}