using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbTrip = ecruise.Database.Models.Trip;

namespace ecruise.Api.Controllers
{
    public class TripsController : BaseController
    {
        // GET: /trips
        [HttpGet(Name = "GetAllTrips")]
        public IActionResult GetAll()
        {
            // create a list of all trips
            ImmutableList<DbTrip> trips = Context.Trips
                // query only trips the current customer has access to
                .Where(t => t.CustomerId == AuthenticatedCustomerId || AuthenticatedCustomerId == 1)
                .ToImmutableList();

            // return 203 No Content if there are no trips
            if (trips.Count == 0)
                return NoContent();

            return Ok(TripAssembler.AssembleModelList(trips));
        }

        // GET: /trips/1
        [HttpGet("{id}", Name = "GetTrip")]
        public async Task<IActionResult> GetOne(ulong id)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested trip
            DbTrip trip = await Context.Trips.FindAsync(id);

            // return error if trip was not found
            if (trip == null)
                return NotFound(new Error(201, "Trip with requested id does not exist.",
                    $"There is no trip that has the id {id}."));

            // forbid if current customer is accessing a different user's trip
            if (!HasAccess(trip.CustomerId))
                return Forbid();

            return Ok(TripAssembler.AssembleModel(trip));
        }

        // POST: /trips
        [HttpPost(Name = "CreateTrip")]
        public async Task<IActionResult> Post([FromBody] Trip trip)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // forbid if current customer is creating a different user's trip
            if (!HasAccess(trip.CustomerId))
                return Forbid();

            // create db trip to be inserted
            DbTrip insertTrip = TripAssembler.AssembleEntity(0, trip);

            // insert trip into database
            var inserted = await Context.Trips.AddAsync(insertTrip);

            await Context.SaveChangesAsync();

            return Created($"{BasePath}/trips/{inserted.Entity.TripId}",
                new PostReference((uint)inserted.Entity.TripId, $"{BasePath}/trips/{inserted.Entity.TripId}"));
        }

        // PATCH: /trips/1
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(ulong id, [FromBody] TripUpdate trip)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested trip
            DbTrip dbtrip = await Context.Trips.FindAsync(id);

            // return error if trip was not found
            if (dbtrip == null)
                return NotFound(new Error(201, "Trip with requested id does not exist.",
                    $"There is no trip that has the id {id}."));

            // forbid if current customer is accessing a different user's trip
            if (!HasAccess(dbtrip.CustomerId))
                return Forbid();

            // update trip end data using a transaction
            using (var transaction = Context.Database.BeginTransaction())
            {
                dbtrip.EndDate = DateTime.UtcNow;
                dbtrip.EndChargingStationId = trip.EndChargingStationId;
                dbtrip.DistanceTravelled = trip.DistanceTravelled;

                transaction.Commit();
                await Context.SaveChangesAsync();
            }

            return Ok(new PostReference(id, $"{BasePath}/trips/{id}"));
        }

        // GET: /trips/by-car/5
        [HttpGet("by-car/{carId}", Name = "GetBookingsByCar")]
        public async Task<IActionResult> GetByCarId(ulong carId)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            List<DbTrip> trips = await Context.Trips
                // query only trips the current customer has access to
                .Where(t => t.CustomerId == AuthenticatedCustomerId || AuthenticatedCustomerId == 1)
                // filter trips by car
                .Where(t => t.CarId == carId)
                .ToListAsync();

            // return 203 No Content if there are no matching trips
            if (trips.Count == 0)
                return NoContent();

            return Ok(TripAssembler.AssembleModelList(trips));
        }
    }
}
