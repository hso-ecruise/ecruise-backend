using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using DbTrip = ecruise.Database.Models.Trip;
using DbInvoice = ecruise.Database.Models.Invoice;
using DbInvoiceItem = ecruise.Database.Models.InvoiceItem;
using DbCarMaintenance = ecruise.Database.Models.CarMaintenance;
using Trip = ecruise.Models.Trip;

namespace ecruise.Api.Controllers
{
    public class TripsController : BaseController
    {
        public TripsController(EcruiseContext context) : base(context)
        {
        }

        // GET: /trips
        [HttpGet(Name = "GetAllTrips")]
        public async Task<IActionResult> GetAll()
        {
            // create a list of all trips
            List<DbTrip> trips = await Context.Trips
                // query only trips the current customer has access to
                .Where(t => t.CustomerId == AuthenticatedCustomerId || AuthenticatedCustomerId == 1)
                .ToListAsync();

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
                return Unauthorized();

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
                return Unauthorized();

            // create db trip to be inserted
            DbTrip insertTrip = TripAssembler.AssembleEntity(0, trip);


            // Check if the car is found and fully loaded
            var car = await Context.Cars.FindAsync(insertTrip.CarId);

            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no car that has the id {insertTrip.CarId}."));

            if (car.ChargingState != "FULL")
                return StatusCode(StatusCodes.Status409Conflict, new Error(303,
                    "The action is not allowed with this resource",
                    "You were trying to use a non fully loaded car for a trip. Cars must be fully loaded to use for a trip"));

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
                return Unauthorized();

            // update trip end data using a transaction
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                dbtrip.EndDate = DateTime.UtcNow;
                dbtrip.EndChargingStationId = trip.EndChargingStationId;
                dbtrip.DistanceTravelled = trip.DistanceTravelled;

                // Get last invoice for the customer (means the invoice of the current month)
                DbInvoice matchingInvoice = Context.Invoices.OrderBy(i => i.InvoiceId).LastOrDefault(i => i.CustomerId == AuthenticatedCustomerId);

                double calculatedAmount = trip.DistanceTravelled * 0.15 +
                                          2.40 * (dbtrip.EndDate.Value - dbtrip.StartDate).TotalHours;

                // Check if invoice found
                if (matchingInvoice == null)
                {
                    // Create new invoice
                    DbInvoice newInvoice = new DbInvoice
                    {
                        CustomerId = AuthenticatedCustomerId,
                        Paid = false,
                        TotalAmount = 0.0
                    };

                    var insert = await Context.Invoices.AddAsync(newInvoice);

                    await Context.SaveChangesAsync();

                    matchingInvoice = insert.Entity;
                }

                // Add the invoice item amount to the total amount of the invoice
                matchingInvoice.TotalAmount += calculatedAmount;

                // Create invoice item for finished booking
                DbInvoiceItem newInvoiceItem = new DbInvoiceItem
                {
                    Reason = "Trip",
                    Type = "DEBIT",
                    // € 0.15 per kilometer + € 2.40 per hour
                    Amount = calculatedAmount,
                    InvoiceId = matchingInvoice.InvoiceId
                };

                // Add invoice item to database
                var insertedInvoiceItem = await Context.InvoiceItems.AddAsync(newInvoiceItem);
                await Context.SaveChangesAsync();

                // Get booking for trip
                var matchingBooking = await Context.Bookings.Where(b => b.TripId == dbtrip.TripId).ToListAsync();
                var booking = matchingBooking.FirstOrDefault();

                if (matchingBooking.Count == 0 || booking == null)
                {
                    return NotFound(new Error(201, "The matching booking of the trip was not found.",
                        $"There is no booking that has the trip id {dbtrip.TripId}."));
                }

                booking.InvoiceItemId = insertedInvoiceItem.Entity.InvoiceItemId;


                // Check if the car must be locked due to a pending maintenance
                // Check if there is a maintenance for the car
                var carMaintenancesForCar = await Context.CarMaintenances.Where(cm => cm.CarId == dbtrip.CarId && !cm.CompletedDate.HasValue)
                    .ToListAsync();

                if (carMaintenancesForCar.Count > 0)
                {
                    // Get the car
                    var car = await Context.Cars.FindAsync(dbtrip.CarId);

                    // Get average time for a trip
                    var allTrips = await Context.Trips.Where(t => t.EndDate.HasValue && t.DistanceTravelled.HasValue).ToListAsync();
                    // ReSharper disable once PossibleInvalidOperationException
                    var averageTripDuration = allTrips.Average(t => t.EndDate.Value.Ticks - t.StartDate.Ticks);

                    // Get average mileage per trip
                    // ReSharper disable once PossibleInvalidOperationException
                    var averageTripMileage = allTrips.Average(t => t.DistanceTravelled.Value);


                    // Sort the maintenances for the car into lists
                    List<DbCarMaintenance> carMaintenancesWithDate = new List<DbCarMaintenance>();
                    carMaintenancesWithDate.AddRange(carMaintenancesForCar.Where(cm => cm.PlannedDate.HasValue || cm.Maintenance.AtDate.HasValue));

                    List<DbCarMaintenance> carMaintenancesWithMileage = new List<DbCarMaintenance>();
                    carMaintenancesWithMileage.AddRange(carMaintenancesForCar.Where(cm => cm.Maintenance.AtMileage.HasValue));

                    var nextMaintenanceByDate = carMaintenancesWithDate.OrderBy(
                        cm => cm.PlannedDate.HasValue && cm.Maintenance.AtDate.HasValue
                            ? DateTime.Compare(cm.PlannedDate.Value, cm.Maintenance.AtDate.Value)
                            : cm.PlannedDate?.Ticks ?? cm.Maintenance.AtDate.Value.Ticks).FirstOrDefault();

                    // Check if there is a maintenance by date close
                    if (nextMaintenanceByDate != null)
                    {
                        DateTime nextBookingDate = nextMaintenanceByDate.PlannedDate.HasValue &&
                                                    nextMaintenanceByDate.Maintenance.AtDate.HasValue
                            ? (nextMaintenanceByDate.PlannedDate.Value <
                               nextMaintenanceByDate.Maintenance.AtDate.Value
                                ? nextMaintenanceByDate.PlannedDate.Value
                                : nextMaintenanceByDate.Maintenance.AtDate.Value)
                            // ReSharper disable once PossibleInvalidOperationException
                            : nextMaintenanceByDate.PlannedDate ?? nextMaintenanceByDate.Maintenance.AtDate.Value;

                        // Check if the car if booked immediately would overdue the next maintenance (average time * 2 because the car would need to be charged first)
                        if (DateTime.UtcNow + new TimeSpan((long)(averageTripDuration * 2)) > nextBookingDate)
                        {
                            // Set the booking state to be blocked
                            car.BookingState = "BLOCKED";
                        }
                    }

                    // Sort the list with mileage by milage due and get the first one
                    var nextMaintenanceByMileage = carMaintenancesWithMileage
                        // ReSharper disable once PossibleInvalidOperationException
                        .OrderBy(cm => cm.Maintenance.AtMileage.Value)
                        .FirstOrDefault();

                    // Check if the next trip would make the car have more mileage than needed for the maintenance (+ 0.5 averageMileage to be sure)
                    // ReSharper disable once PossibleInvalidOperationException
                    if (car.Milage + averageTripMileage * 1.5 >
                        nextMaintenanceByMileage?.Maintenance.AtMileage)
                    {
                        // Set the booking state to be blocked
                        car.BookingState = "BLOCKED";
                    }
                }

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
