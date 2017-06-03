using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
using ecruise.Models.Assemblers;
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

            return Ok(TripAssembler.AssembleModelList(trips));
        }

        // POST: /trips
        [HttpPost(Name = "CreateTrip")]
        public IActionResult Post([FromBody] Trip trip)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            var insertTrip = TripAssembler.AssembleEntity(trip);

            var inserted = Context.Trips.Add(insertTrip);

            return Created($"{BasePath}/trips/{inserted.Entity.TripId}",
                new PostReference((uint)inserted.Entity.TripId, $"{BasePath}/trips/{inserted.Entity.TripId}"));
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
