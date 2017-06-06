using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Get()
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
        public async Task<IActionResult> Get(ulong id)
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
        public async Task<IActionResult> Post([FromBody] CarMaintenance carMaintenance)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            if (ModelState.IsValid)
            {
                // Check car maintenace for logical validity
                // Check car
                if (Context.Customers.Find((ulong)carMaintenance.CarId) == null)
                    return NotFound(new Error(202, "The car referenced in the given object does not exist.",
                        "The referenced car must already exist to create a new car maintenance."));

                // Check maintenance
                if (Context.Customers.Find((ulong)carMaintenance.MaintenanceId) == null)
                    return NotFound(new Error(202, "The maintenance referenced in the given object does not exist.",
                        "The referenced maintenace must already exist to create a new car maintenance."));

                // Check invoice item if set
                if (carMaintenance.InvoiceItemId != null)
                    if (Context.Customers.Find((ulong)carMaintenance.MaintenanceId) == null)
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

        // PATCH: /CarMaintenances/5/completed-date
        [HttpPatch("{id}/completed-date")]
        public async Task<IActionResult> Patch(ulong id, [FromBody] string date)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Transform string to date
            DateTime newCompletedDateTime;
            if (DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out newCompletedDateTime))
            {
                // Check given date for logical validity
                if (newCompletedDateTime.ToUniversalTime() > DateTime.UtcNow)
                    return BadRequest(new Error(302, "Completed date must be in the past.",
                        "The given date wasn't set properly. Please check the message for further information."));

                // Get the specified car maintenance
                var carMaintenance = await Context.CarMaintenances.FindAsync(id);

                if (carMaintenance == null)
                    return NotFound(new Error(201, "A car maintenance with requested id does not exist.",
                        "An error occured. Please check the message for further information."));

                // Patch completed date and save the change
                carMaintenance.CompletedDate = newCompletedDateTime;
                await Context.SaveChangesAsync();

                // Return a reference to the patch object
                return Ok(new PostReference(id, $"{BasePath}/carmaintenances/{id}"));
            }

            return BadRequest(new Error(301, "The date given was not formatted correctly.",
                "Date must always be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'"));
        }

        // GET: /CarMaintenances/by-car/5
        [HttpGet("by-car/{id}", Name = "GetCarMaintenancesByCar")]
        public async Task<IActionResult> GetByCarId(ulong id)
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
        public async Task<IActionResult> GetByMaintenanceId(ulong id)
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
        public async Task<IActionResult> GetByInvoiceItemId(ulong id)
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
