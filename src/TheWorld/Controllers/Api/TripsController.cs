using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips")]
    public class TripsController : Controller
    {
        private ILogger<TripsController> _logger;
        private IWorldRepository _repository;

        public TripsController(IWorldRepository repository, ILogger<TripsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var results = _repository.GetTripsByUsername(this.User.Identity.Name);
                return Ok(Mapper.Map<IEnumerable<TripViewModel>>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex, "Failed to get Trips");

                return BadRequest("Error occurred");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TripViewModel theTrip)
        {
            if (ModelState.IsValid)
            {
                var newTrip = Mapper.Map<Trip>(theTrip);

                newTrip.UserName = User.Identity.Name;

                _repository.AddTrip(newTrip);

                if (await _repository.SaveChangesAsync())
                {
                    return Created($"api/trips/{newTrip.Name}", Mapper.Map<TripViewModel>(newTrip));
                }
            }
            
            return BadRequest("Failed to add new Trip");
        }
    }
}
