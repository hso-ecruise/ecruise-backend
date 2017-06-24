using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarMaintenance = ecruise.Models.CarMaintenance;

namespace ecruise.Api.Controllers
{
    [Route("v1/car-maintenances")]
    public class CarMaintenancesController : BaseController
    {
        public CarMaintenancesController(EcruiseContext context) : base(context)
        {
        }

        // GET: CarMaintenances
        [HttpGet(Name = "GetAllCarMaintenances")]
        public async Task<IActionResult> GetAsync()
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Get all entities from database
            var carMaintenances = await Context.CarMaintenances.ToListAsync();

            if (carMaintenances.Count < 1)
                // Return that there are no results
                return NoContent();

            // Return found car maintenances
            return Ok(CarMaintenanceAssembler.AssembleModelList(carMaintenances));
        }

        // GET: /CarMaintenances/5
        [HttpGet("{id}", Name = "GetCarMaintenance")]
        public async Task<IActionResult> GetAsync(ulong id)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Check for correct value
            if (!ModelState.IsValid)
                return BadRequest(new Error(301,
                    "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));

            // Get booking from database
            var carMaintenance = await Context.CarMaintenances.FindAsync(id);

            if (carMaintenance != null)
                return Ok(CarMaintenanceAssembler.AssembleModel(carMaintenance));

            return NotFound(new Error(201, "A car maintenance with requested id does not exist.",
                "An error occured. Please check the message for further information."));
        }

        // POST: CarMaintenances
        [HttpPost]
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public async Task<IActionResult> PostAsync([FromBody] CarMaintenance carMaintenance)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            if (ModelState.IsValid)
            {
                // Check car maintenace for logical validity
                // Check car
                var car = Context.Cars.Find((ulong)carMaintenance.CarId);

                if (car == null)
                    return NotFound(new Error(202, "The car referenced in the given object does not exist.",
                        "The referenced car must already exist to create a new car maintenance."));

                // Check maintenance
                var maintenance = Context.Maintenances.Find((ulong)carMaintenance.MaintenanceId);

                if (maintenance == null)
                    return NotFound(new Error(202, "The maintenance referenced in the given object does not exist.",
                        "The referenced maintenace must already exist to create a new car maintenance."));

                // Check invoice item if set
                if (carMaintenance.InvoiceItemId != null)
                    if (Context.InvoiceItems.Find((ulong)carMaintenance.InvoiceItemId) == null)
                        return NotFound(new Error(202,
                            "The invoice item referenced in the given object does not exist.",
                            "The referenced invoice item does not have to exist to create a new car maintenance."));

                // Check the dates if set
                if (carMaintenance.PlannedDate.HasValue)
                    if (carMaintenance.PlannedDate.Value.ToUniversalTime() < DateTime.UtcNow)
                        return BadRequest(new Error(302, "Planned date must be in the future",
                            "Maintenances cannot be planned for the past."));

                if (carMaintenance.CompletedDate.HasValue)
                    if (carMaintenance.CompletedDate.Value.ToUniversalTime() > DateTime.UtcNow)
                        return BadRequest(new Error(302, "Completed date must be in the past",
                            "Maintenances can only be completed, when they are completed but not in advance"));

                // Construct entity from model
                Database.Models.CarMaintenance carMaintenanceEntity =
                    CarMaintenanceAssembler.AssembleEntity(0, carMaintenance);

                // Save to database
                await Context.CarMaintenances.AddAsync(carMaintenanceEntity);

                // Check the car has to be blocked immediately
                // Check if it is in use right now
                if (car.BookingState == "AVAILABLE")
                {
                    // Get average time for a trip
                    var allTrips = await Context.Trips.Where(t => t.EndDate.HasValue && t.DistanceTravelled.HasValue)
                        .ToListAsync();

                    if (maintenance.Spontaneously)
                    {
                        car.BookingState = "BLOCKED";
                    }
                    else
                    {
                        // Check if car must be blocked by date
                        if (maintenance.AtDate.HasValue)
                        {
                            // Get average duration of trip
                            var averageTripDuration = allTrips.Average(t => t.EndDate.Value.Ticks - t.StartDate.Ticks);

                            if (DateTime.UtcNow + new TimeSpan((long)(averageTripDuration * 2)) > maintenance.AtDate)
                                car.BookingState = "BLOCKED";
                        }
                        // Check if car needs to be blocked by mileage
                        if (maintenance.AtMileage.HasValue)
                        {
                            // Get average mileage per trip
                            var averageTripMileage = allTrips.Average(t => t.DistanceTravelled.Value);

                            // Check if the next trip would make the car have more mileage than needed for the maintenance (+ 0.5 averageMileage to be sure)
                            if (car.Milage + averageTripMileage * 1.5 > maintenance.AtMileage)
                                car.BookingState = "BLOCKED";
                        }
                    }
                }

                await Context.SaveChangesAsync();

                // Get the reference to the newly created entity
                PostReference pr = new PostReference((uint)carMaintenanceEntity.CarMaintenanceId,
                    $"{BasePath}/carmaintenances/{carMaintenanceEntity.CarMaintenanceId}");

                // Return reference to the new object including the path to it
                return Created($"{BasePath}/carmaintenances/{carMaintenanceEntity.CarMaintenanceId}", pr);
            }

            return BadRequest(new Error(301, GetModelStateErrorString(),
                "The given data could not be converted to a car maintenance object. Please check the message for further information."));
        }

        // PATCH: /CarMaintenances/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(ulong id, [FromBody] CarMaintenanceUpdate carMaintenanceUpdate)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            if (ModelState.IsValid)
            {
                // Check given date for logical validity
                if (carMaintenanceUpdate.CompletedDate.ToUniversalTime() > DateTime.UtcNow)
                    return BadRequest(new Error(302, "Completed date must be in the past.",
                        "The given date wasn't set properly. Please check the message for further information."));

                // Check if the invoice item exists
                if (await Context.InvoiceItems.FindAsync((ulong)carMaintenanceUpdate.InvoiceItemId) == null)
                    return NotFound(new Error(201, "A invoice item with requested id does not exist.",
                        "An error occured. Please check the message for further information."));

                // Get the specified car maintenance
                var carMaintenance = await Context.CarMaintenances.FindAsync(id);

                if (carMaintenance == null)
                    return NotFound(new Error(201, "A car maintenance with requested id does not exist.",
                        "An error occured. Please check the message for further information."));

                // Patch car maintenance object
                carMaintenance.CompletedDate = carMaintenanceUpdate.CompletedDate;
                carMaintenance.InvoiceItemId = carMaintenanceUpdate.InvoiceItemId;

                // Save the changes
                await Context.SaveChangesAsync();

                // Return a reference to the patch object
                return Ok(new PostReference(id, $"{BasePath}/carmaintenances/{id}"));
            }

            return BadRequest(new Error(301, GetModelStateErrorString(),
                "The given data could not be converted to a car maintenance update object. Please check the message for further information."));
        }

        // GET: /CarMaintenances/by-car/5
        [HttpGet("by-car/{id}", Name = "GetCarMaintenancesByCar")]
        public async Task<IActionResult> GetByCarIdAsync(ulong id)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            if (ModelState.IsValid)
            {
                // Get all entities with the given car id
                var carMaintenanceEntities = await Context.CarMaintenances
                    .Where(cm => cm.CarId == id)
                    .ToListAsync();

                if (carMaintenanceEntities.Count < 1)
                    return NoContent();

                // Convert them to models and return OK
                return Ok(CarMaintenanceAssembler.AssembleModelList(carMaintenanceEntities));
            }

            return BadRequest(new Error(301, GetModelStateErrorString(),
                "The given id could not be converted. Please check the message for further information."));
        }

        // GET: /CarMaintenances/by-maintenance/5
        [HttpGet("by-maintenance/{id}", Name = "GetCarMaintenancesByMaintenance")]
        public async Task<IActionResult> GetByMaintenanceIdAsync(ulong id)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            if (ModelState.IsValid)
            {
                // Get all entities with the given car id
                var carMaintenanceEntities = await Context.CarMaintenances
                    .Where(cm => cm.MaintenanceId == id)
                    .ToListAsync();

                if (carMaintenanceEntities.Count < 1)
                    return NoContent();

                // Convert them to models and return OK
                return Ok(CarMaintenanceAssembler.AssembleModelList(carMaintenanceEntities));
            }

            return BadRequest(new Error(301, GetModelStateErrorString(),
                "The given id could not be converted. Please check the message for further information."));
        }

        // GET: /CarMaintenances/by-invoice-item/5
        [HttpGet("by-invoice-item/{id}", Name = "GetCarMaintenanceByInvoiceItem")]
        public async Task<IActionResult> GetByInvoiceItemIdAsync(ulong id)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            if (ModelState.IsValid)
            {
                // Get all entities with the given car id
                var carMaintenanceEntities = await Context.CarMaintenances
                    .Where(cm => cm.InvoiceItemId.HasValue && cm.InvoiceItemId.Value == id).ToListAsync();

                if (carMaintenanceEntities.Count < 1)
                    return NoContent();

                // Convert them to models and return OK
                return Ok(CarMaintenanceAssembler.AssembleModelList(carMaintenanceEntities));
            }

            return BadRequest(new Error(301, GetModelStateErrorString(),
                "The given id could not be converted. Please check the message for further information."));
        }
    }
}
