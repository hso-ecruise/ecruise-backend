using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
using DbTrip = ecruise.Database.Models.Trip;

namespace ecruise.Api.Controllers
{
    public class TripsController : BaseController
    {
        // GET: /trips
        [HttpGet(Name = "GetAllTrips")]
        public IActionResult GetAll()
        {
            ImmutableList<DbTrip> trips = Context.Trips.ToImmutableList();
            if (trips.Count == 0)
                return NoContent();

            return Ok(trips);
        }

        // POST: /trips
        [HttpPost(Name = "CreateTrip")]
        public IActionResult Post([FromBody] Trip trip)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbTrip insertTrip = new DbTrip
            {
                CarId = trip.CarId,
                CustomerId = trip.CustomerId,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,

                StartChargingStationId = trip.StartChargingStationId,
                EndChargingStationId = trip.EndChargingStationId,

                // whole entities (?)
                Car = Context.Cars.Find(trip.CarId),
                Customer = Context.Customers.Find(trip.CustomerId),

                StartChargingStation = Context.ChargingStations.Find(trip.StartChargingStationId),
                EndChargingStation = Context.ChargingStations.Find(trip.EndChargingStationId)
            };

            var inserted = Context.Trips.Add(insertTrip);

            return Created($"{BasePath}/trips/{inserted.Entity.TripId}",
                new PostReference(inserted.Entity.TripId, $"{BasePath}/trips/{inserted.Entity.TripId}"));
        }

        // GET: /trips/1
        [HttpGet("{id}", Name = "GetTrip")]
        public IActionResult GetOne(uint id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbTrip trip = Context.Trips.Find(id);
            if (trip == null)
                return NotFound(new Error(201, "Trip with requested id does not exist.",
                    $"There is no trip that has the id {id}."));

            return Ok(trip);
        }

        // PATCH: /trips/1
        [HttpPatch("{id}")]
        public IActionResult Patch(uint id, [FromBody] TripUpdate trip)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbTrip dbtrip = Context.Trips.Find(id);
            if (dbtrip == null)
                return NotFound(new Error(201, "Trip with requested id does not exist.",
                    $"There is no trip that has the id {id}."));

            using (var transaction = Context.Database.BeginTransaction())
            {
                dbtrip.EndChargingStationId = trip.EndChargingStationId;
                dbtrip.DistanceTravelled = trip.DistanceTravelled;

                // update entity (?)
                dbtrip.EndChargingStation = Context.ChargingStations.Find(trip.EndChargingStationId);

                transaction.Commit();
            }

            return Ok(new PostReference(id, $"{BasePath}/trips/{id}"));
        }

        // GET: /trips/by-car/5
        [HttpGet("by-car/{carId}", Name = "GetBookingsByCar")]
        public IActionResult GetByCarId(uint carId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            ImmutableList<DbTrip> trips = Context.Trips
                .Where(t => t.CarId == carId)
                .ToImmutableList();

            if (trips.Count == 0)
                return NoContent();

            return Ok(trips);
        }
    }
}
