using System;
using System.Collections.Generic;
using ecruise.Models;
using Microsoft.AspNetCore.Mvc;

namespace ecruise.Api.Controllers
{
    public class TripsController : BaseController
    {
        // GET: /trips
        [HttpGet(Name = "GetAllTrips")]
        public IActionResult GetAll()
        {
            Trip t1 = new Trip(1, null, 3, null, null, null, null, null);
            Trip t2 = new Trip(2, 6, 3, new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc), null, 18, null, null);
            Trip t3 = new Trip(3, 6, 3, new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc),
                new DateTime(2017, 5, 8, 14, 42, 0, DateTimeKind.Utc), 18, 7, 7412);

            return Ok(new List<Trip> {t1, t2, t3});
        }

        // POST: /trips
        [HttpPost(Name = "CreateTrip")]
        public IActionResult Post([FromBody] Trip trip)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/trips/1",
                    new PostReference(trip.TripId, $"{BasePath}/trips/1"));
            else
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
        }

        // GET: /trips/1
        [HttpGet("{id}", Name = "GetTrip")]
        public IActionResult Get(uint id)
        {
            if (ModelState.IsValid && id < 3)
            {
                Trip trip = new Trip(id, 6, 3, new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc),
                    new DateTime(2017, 5, 8, 14, 42, 0, DateTimeKind.Utc), 18, 7, 2594);

                return Ok(trip);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound(new Error(1, "Trip with requested trip id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // PATCH: /trips/1
        [HttpPatch("{id}")]
        public IActionResult Patch(uint id, [FromBody] TripUpdate trip)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Ok(new PostReference(id, $"{BasePath}/trips/{id}"));
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "Trip with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /trips/by-car/5
        [HttpGet("by-car/{carId}", Name = "GetBookingsByCar")]
        public IActionResult GetByCarId(uint carId)
        {
            if (ModelState.IsValid && carId < 3 && carId > 0)
            {
                Trip t1 = new Trip(2, carId, 3, new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc),
                    new DateTime(2017, 5, 8, 14, 42, 0, DateTimeKind.Utc), 18, 7, 2594);

                Trip t2 = new Trip(6, carId, 3, new DateTime(2017, 5, 10, 05, 13, 0, DateTimeKind.Utc),
                    new DateTime(2017, 5, 10, 09, 47, 0, DateTimeKind.Utc), 12, 4, 16772);

                return Ok(new List<Trip>{t1, t2});
            }
            else if (ModelState.IsValid && carId >= 3)
            {
                return NotFound("Trip with requested car id does not exist.");
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. CarId must be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }
    }
}
