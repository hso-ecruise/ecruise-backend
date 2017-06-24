using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Booking = ecruise.Models.Booking;
using DbBooking = ecruise.Database.Models.Booking;
using DbCarChargingStation = ecruise.Database.Models.CarChargingStation;
using DbTrip = ecruise.Database.Models.Trip;
using DbInvoice = ecruise.Database.Models.Invoice;
using DbInvoiceItem = ecruise.Database.Models.InvoiceItem;
using DbCarMaintenance = ecruise.Database.Models.CarMaintenance;
using DbMaintenance = ecruise.Database.Models.Maintenance;
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
        public async Task<IActionResult> GetAllAsync()
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
        public async Task<IActionResult> GetOneAsync(ulong id)
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
        public async Task<IActionResult> PostAsync([FromBody] Trip trip)
        {
            // Forbid if current customer is creating a different user's trip
            if (!HasAccess())
                return Unauthorized();

            // Validate user input
            if (trip == null || !ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // Check if the user is allowed to make bookings
            var dbCustomer = await Context.Customers.FindAsync((ulong)trip.CustomerId);

            if (dbCustomer.Activated == false || dbCustomer.Verified == false)
                return Unauthorized();

            // Check if new bookings are allowed
            var config = await Context.Configurations.FindAsync((ulong)1);

            if (config == null || config.AllowNewBookings == false)
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new Error(501, "Currently are no new bookings allowed",
                        "An error occured.Please check the message for further information."));

            // Create db trip to be inserted
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

            if (car.BookingState != "AVAILABLE")
                return StatusCode(StatusCodes.Status409Conflict, new Error(303,
                    "The action is not allowed with this resource",
                    "You were trying to use a car that is currently not available."));

            // Check if the charging station extists
            var chargingStation = await Context.ChargingStations.FindAsync((ulong)trip.StartChargingStationId);

            if (chargingStation == null)
                return NotFound(new Error(201, "Charging station with requested id does not exist.",
                    $"There is no car that has the id {insertTrip.StartChargingStationId}."));

            // Decrement occupied slots of charging station
            chargingStation.SlotsOccupied = chargingStation.SlotsOccupied == 0 ? 0 : chargingStation.SlotsOccupied - 1;

            // Change the charging state to discharging
            car.ChargingState = "DISCHARGING";
            car.BookingState = "BOOKED";
            car.LastKnownPositionDate = DateTime.UtcNow;

            // insert trip into database
            var insertedTrip = await Context.Trips.AddAsync(insertTrip);

            await Context.SaveChangesAsync();

            // Create booking for trip
            Booking booking = new Booking(0, trip.CustomerId, (uint)insertedTrip.Entity.TripId, null,
                chargingStation.Latitude, chargingStation.Longitude, insertedTrip.Entity.StartDate, null);

            DbBooking dbBooking = BookingAssembler.AssembleEntity(0, booking);

            // Add it to the database
            await Context.Bookings.AddAsync(dbBooking);

            await Context.SaveChangesAsync();

            // Send a confirmation mail to customer
            var customer = CustomerAssembler.AssembleModel(dbCustomer);
            try
            {
                await customer.SendMail("eCruise: Beginn deiner Fahrt",
                    $"Hallo {customer.FirstName}!<br/>Du hast gerade eine Fahrt mit einem unserer Fahrzeuge gestartet.<br/>Fahrtbeginn: {insertedTrip.Entity.StartDate:f}<br/>Viel Spaß und Gute Fahrt!<br/><br/>Liebe Grüße<br/>Dein eCruise-Team");
            }
            catch (Exception e)
            {
                Debug.WriteLine(
                    $"Trip with id {insertedTrip.Entity.TripId} created, but email sending to {customer.FirstName} {customer.LastName} with mail address {customer.Email} failed.\nComplete exception message: {e.Message}",
                    "WARNING");
            }

            return Created($"{BasePath}/trips/{insertedTrip.Entity.TripId}",
                new PostReference((uint)insertedTrip.Entity.TripId, $"{BasePath}/trips/{insertedTrip.Entity.TripId}"));
        }

        // PATCH: /trips/1
        [HttpPatch("{id}")]
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public async Task<IActionResult> PatchAsync(ulong id, [FromBody] TripUpdate tripUpdate)
        {
            // validate user input
            if (tripUpdate == null || !ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // forbid if current customer is accessing a different user's trip
            if (!HasAccess())
                return Unauthorized();

            // find the requested trip
            DbTrip dbtrip = await Context.Trips.FindAsync(id);

            // return error if trip was not found
            if (dbtrip == null)
                return NotFound(new Error(201, "Trip with requested id does not exist.",
                    $"There is no trip that has the id {id}."));

            // update trip end data using a transaction
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                dbtrip.EndDate = DateTime.UtcNow;
                dbtrip.EndChargingStationId = tripUpdate.EndChargingStationId;
                dbtrip.DistanceTravelled = tripUpdate.DistanceTravelled;

                // Check if end charging station exists
                var endChargingStation = await Context.ChargingStations.FindAsync(dbtrip.EndChargingStationId);

                if (endChargingStation == null)
                    return NotFound(new Error(201, "Charging station with requested id does not exist.",
                        $"A charging station with id {id} does not exist."));

                // Increment the occupied slots
                if (endChargingStation.SlotsOccupied < endChargingStation.Slots)
                    endChargingStation.SlotsOccupied++;

                // Get last invoice for the customer (means the invoice of the current month)
                DbInvoice matchingInvoice = Context.Invoices.OrderBy(i => i.InvoiceId)
                    .LastOrDefault(i => i.CustomerId == dbtrip.CustomerId);

                double calculatedAmount = tripUpdate.DistanceTravelled * 0.15 +
                                          2.40 * (dbtrip.EndDate.Value - dbtrip.StartDate).TotalHours;

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
                    return NotFound(new Error(201, "The matching booking of the trip was not found.",
                        $"There is no booking that has the trip id {dbtrip.TripId}."));

                booking.InvoiceItemId = insertedInvoiceItem.Entity.InvoiceItemId;

                // Connect the car to the charging station given in endChargingStation
                var carChargingStation = new DbCarChargingStation
                {
                    CarChargingStationId = 0,
                    CarId = (ulong)dbtrip.CarId,
                    ChargeStart = DateTime.UtcNow,
                    ChargingStationId = (ulong)dbtrip.EndChargingStationId
                };

                // Add the new connection to the database
                await Context.CarChargingStations.AddAsync(carChargingStation);

                // Patch the car to have correct values
                // Get the car
                var car = await Context.Cars.FindAsync(dbtrip.CarId);

                car.BookingState = "AVAILABLE";
                car.ChargingState = "CHARGING";
                // Charge level and mileage will be changed separately by the car itself

                // Check if the car must be locked due to a pending maintenance
                // Check if there is a maintenance for the car
                var carMaintenancesForCar = await Context.CarMaintenances
                    .Where(cm => cm.CarId == dbtrip.CarId && !cm.CompletedDate.HasValue)
                    .ToListAsync();

                if (carMaintenancesForCar.Count > 0)
                {
                    // Get average time for a trip
                    var allTrips = await Context.Trips.Where(t => t.EndDate.HasValue && t.DistanceTravelled.HasValue)
                        .ToListAsync();

                    var averageTripDuration = allTrips.Average(t => t.EndDate.Value.Ticks - t.StartDate.Ticks);

                    // Get average mileage per trip
                    var averageTripMileage = allTrips.Average(t => t.DistanceTravelled.Value);

                    // Get all maintenances
                    var allMaintenances = await Context.Maintenances.ToListAsync();

                    // Sort the maintenances for the car into lists
                    List<DbCarMaintenance> carMaintenancesWithDate = new List<DbCarMaintenance>();
                    List<DbMaintenance> maintenancesWithDate = new List<DbMaintenance>();

                    foreach (var carMaintenance in carMaintenancesForCar)
                    {
                        // Add the carMaintenance to the list if the car maintenance has a planned date
                        if (carMaintenance.PlannedDate.HasValue)
                            carMaintenancesWithDate.Add(carMaintenance);

                        // Or the maintenance linked has a date
                        if (!allMaintenances.Any(m => m.MaintenanceId == carMaintenance.MaintenanceId &&
                                                      m.AtDate.HasValue))
                            continue;

                        var maintenance = allMaintenances.FirstOrDefault(
                            m => m.MaintenanceId == carMaintenance.MaintenanceId &&
                                 m.AtDate.HasValue);

                        if (maintenance != null)
                            maintenancesWithDate.Add(maintenance);
                    }

                    // find car maintenance that has an mileage-associated maintenance
                    List<DbCarMaintenance> carMaintenancesWithMileage =
                    (from carMaintenance in carMaintenancesForCar
                     let matchingMaintenance =
                     allMaintenances.FirstOrDefault(m => m.MaintenanceId == carMaintenance.MaintenanceId)
                     where matchingMaintenance?.AtMileage != null
                     select carMaintenance).ToList();

                    DateTime? maintenanceDate = null;

                    // Get next maintenance with date 
                    if (maintenancesWithDate.Count > 0)
                    {
                        var maintenance = maintenancesWithDate.OrderBy(m => m.AtDate.Value).FirstOrDefault();

                        if (maintenance != null)
                            maintenanceDate = maintenance.AtDate.Value;
                    }

                    if (carMaintenancesWithDate.Count > 0)
                    {
                        var maintenance = carMaintenancesWithDate.OrderBy(cm => cm.PlannedDate.Value).FirstOrDefault();

                        // Check if the date is earlier than the set date
                        if (maintenanceDate == null || maintenanceDate > maintenance.PlannedDate.Value)
                            maintenanceDate = maintenance.PlannedDate;
                    }

                    // Check if there is a maintenance by date close
                    if (maintenanceDate != null)
                        if (DateTime.UtcNow + new TimeSpan((long)(averageTripDuration * 2)) > maintenanceDate)
                            car.BookingState = "BLOCKED";

                    // Find maintenance that relates to car maintenance
                    List<DbMaintenance> maintenancesWithMileage =
                        carMaintenancesWithMileage
                            .Select(cm => allMaintenances.Find(m => m.MaintenanceId == cm.MaintenanceId))
                            .ToList();

                    // Walk through all saves carMaintenandesWithMileage
                    var firstMaintenanceByMileage = maintenancesWithMileage.OrderBy(m => m.AtMileage).FirstOrDefault();

                    // Check if the next trip would make the car have more mileage than needed for the maintenance (+ 0.5 averageMileage to be sure)
                    if (car.Milage + averageTripMileage * 1.5 > firstMaintenanceByMileage?.AtMileage)
                        car.BookingState = "BLOCKED";
                }

                transaction.Commit();
                await Context.SaveChangesAsync();
            }

            return Ok(new PostReference(id, $"{BasePath}/trips/{id}"));
        }

        // GET: /trips/by-car/5
        [HttpGet("by-car/{carId}", Name = "GetBookingsByCar")]
        public async Task<IActionResult> GetByCarIdAsync(ulong carId)
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
