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

        // GET: /trips/1
        [HttpGet("{id}", Name = "GetTrip")]
        public IActionResult GetOne(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbTrip trip = Context.Trips.Find(id);
            if (trip == null)
                return NotFound(new Error(201, "Trip with requested id does not exist.",
                    $"There is no trip that has the id {id}."));

            return Ok(TripAssembler.AssembleModel(trip));
        }

        // POST: /trips
        [HttpPost(Name = "CreateTrip")]
        public IActionResult Post([FromBody] Trip trip)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            var insertTrip = TripAssembler.AssembleEntity(0, trip);

            var inserted = Context.Trips.Add(insertTrip);
            Context.SaveChanges();

            return Created($"{BasePath}/trips/{inserted.Entity.TripId}",
                new PostReference((uint)inserted.Entity.TripId, $"{BasePath}/trips/{inserted.Entity.TripId}"));
        }

        // PATCH: /trips/1
        [HttpPatch("{id}")]
        public IActionResult Patch(ulong id, [FromBody] TripUpdate trip)
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
                dbtrip.EndDate = System.DateTime.UtcNow;
                dbtrip.EndChargingStationId = trip.EndChargingStationId;
                dbtrip.DistanceTravelled = trip.DistanceTravelled;

                transaction.Commit();
                Context.SaveChangesAsync();
            }

            return Ok(new PostReference(id, $"{BasePath}/trips/{id}"));
        }

        // GET: /trips/by-car/5
        [HttpGet("by-car/{carId}", Name = "GetBookingsByCar")]
        public IActionResult GetByCarId(ulong carId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            ImmutableList<DbTrip> trips = Context.Trips
                .Where(t => t.CarId == carId)
                .ToImmutableList();

            if (trips.Count == 0)
                return NoContent();

            return Ok(TripAssembler.AssembleModelList(trips));
        }
    }
}
