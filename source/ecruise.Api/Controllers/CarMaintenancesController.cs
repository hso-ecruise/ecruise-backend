using System.Collections.Immutable;
using System.Globalization;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ecruise.Api.Controllers
{
    [Route("v1/car-maintenances")]
    public class CarMaintenancesController : BaseController
    {
        // GET: CarMaintenances
        [HttpGet(Name = "GetAllCarMaintenances")]
        public IActionResult Get()
        {
            try
            {
                // Get all entities from database
                var carMaintenances = Context.CarMaintenances.ToImmutableList();

                if (carMaintenances.Count < 1)
                    // Return that there are no results
                    return NoContent();

                else
                {
                    // Return found car maintenances
                    return Ok(CarMaintenanceAssembler.AssembleModelList(carMaintenances));
                }
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // GET: /CarMaintenances/5
        [HttpGet("{id}", Name = "GetCarMaintenance")]
        public IActionResult Get(ulong id)
        {
            try
            {
                // Check for correct value
                if (!ModelState.IsValid)
                    return BadRequest(new Error(301, "The id given was not formatted correctly. Id must be unsigned int",
                        "An error occured. Please check the message for further information."));

                // Get booking from database
                var carMaintenance = Context.CarMaintenances.Find(id);

                if (carMaintenance != null)
                    return Ok(CarMaintenanceAssembler.AssembleModel(carMaintenance));

                else
                    return NotFound(new Error(201, "A car maintenance with requested id does not exist.",
                        "An error occured. Please check the message for further information."));
            }
            catch(Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // POST: CarMaintenances
        [HttpPost]
        public IActionResult Post([FromBody]CarMaintenance carMaintenance)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check car maintenace for logical validity
                    // Check car
                    if (Context.Customers.Find(carMaintenance.CarId) == null)
                        return NotFound(new Error(202, "The car referenced in the given object does not exist.",
                            "The referenced car must already exist to create a new car maintenance."));

                    // Check maintenance
                    if (Context.Customers.Find(carMaintenance.MaintenanceId) == null)
                        return NotFound(new Error(202, "The maintenance referenced in the given object does not exist.",
                            "The referenced maintenace must already exist to create a new car maintenance."));

                    // Check invoice item if set
                    if(carMaintenance.InvoiceItemId != null)
                        if (Context.Customers.Find(carMaintenance.MaintenanceId) == null)
                            return NotFound(new Error(202, "The invoice item referenced in the given object does not exist.",
                                "The referenced invoice item does not have to exist to create a new car maintenance."));

                    // Check the dates if set
                    if(carMaintenance.PlannedDate.HasValue)
                        if(carMaintenance.PlannedDate.Value.ToUniversalTime() < DateTime.UtcNow)
                            return BadRequest(new Error(302, "Planned date must be in the future",
                                "Maintenances cannot be planned for the past."));

                    if (carMaintenance.CompletedDate.HasValue)
                        if (carMaintenance.CompletedDate.Value.ToUniversalTime() > DateTime.UtcNow)
                            return BadRequest(new Error(302, "Completed date must be in the past",
                                "Maintenances can only be completed, when they are completed but not in advance"));

                    // Construct entity from model
                    Database.Models.CarMaintenance carMaintenanceEntity = CarMaintenanceAssembler.AssembleEntity(0, carMaintenance);

                    // Save to database
                    Context.CarMaintenances.Add(carMaintenanceEntity);
                    Context.SaveChanges();

                    // Get the reference to the newly created entity
                    PostReference pr = new PostReference((uint)carMaintenanceEntity.CarMaintenanceId,
                        $"{BasePath}/carmaintenances/{carMaintenanceEntity.CarMaintenanceId}");

                    // Return reference to the new object including the path to it
                    return Created($"{BasePath}/carmaintenances/{carMaintenanceEntity.CarMaintenanceId}", pr);
                }
                else
                    return BadRequest(new Error(301, GetModelStateErrorString(),
                        "The given data could not be converted to a car maintenance object. Please check the message for further information."));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }
                
        // PATCH: /CarMaintenances/5/completed-date
        [HttpPatch("{id}/completed-date")]
        public IActionResult Patch(ulong id, [FromBody] string date)
        {
            // Transform string to date
            DateTime newCompletedDateTime;
            if (DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out newCompletedDateTime))
            {
                // Check given date for logical validity
                if(newCompletedDateTime.ToUniversalTime() > DateTime.UtcNow)
                    return BadRequest(new Error(302, "Completed date must be in the past.",
                        "The given date wasn't set properly. Please check the message for further information."));

                // Get the specified car maintenance
                var carMaintenance = Context.CarMaintenances.Find(id);

                if(carMaintenance == null)
                    return NotFound(new Error(201, "A car maintenance with requested id does not exist.",
                        "An error occured. Please check the message for further information."));

                // Patch completed date and save the change
                carMaintenance.CompletedDate = newCompletedDateTime;
                Context.SaveChangesAsync();

                // Return a reference to the patch object
                return Ok(new PostReference(id, $"{BasePath}/carmaintenances/{id}"));
            }
            else
            {
                return BadRequest(new Error(301, "The date given was not formatted correctly.",
                    "Date must always be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'"));
            }
        }

        // GET: /CarMaintenances/by-car/5
        [HttpGet("by-car/{id}", Name = "GetCarMaintenancesByCar")]
        public IActionResult GetByCarId(ulong id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get all entities with the given car id
                    var carMaintenanceEntities = Context.CarMaintenances.Where(cm => cm.CarId == id).ToImmutableList();

                    if (carMaintenanceEntities.Count < 1)
                        return NoContent();

                    // Convert them to models and return OK
                    return Ok(CarMaintenanceAssembler.AssembleModelList(carMaintenanceEntities));
                }
                else
                    return BadRequest(new Error(301, GetModelStateErrorString(),
                        "The given id could not be converted. Please check the message for further information."));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // GET: /CarMaintenances/by-maintenance/5
        [HttpGet("by-maintenance/{id}", Name = "GetCarMaintenancesByMaintenance")]
        public IActionResult GetByMaintenanceId(ulong id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get all entities with the given car id
                    var carMaintenanceEntities = Context.CarMaintenances.Where(cm => cm.MaintenanceId == id).ToImmutableList();

                    if (carMaintenanceEntities.Count < 1)
                        return NoContent();

                    // Convert them to models and return OK
                    return Ok(CarMaintenanceAssembler.AssembleModelList(carMaintenanceEntities));
                }
                else
                    return BadRequest(new Error(301, GetModelStateErrorString(),
                        "The given id could not be converted. Please check the message for further information."));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // GET: /CarMaintenances/by-invoice-item/5
        [HttpGet("by-invoice-item/{id}", Name = "GetCarMaintenanceByInvoiceItem")]
        public IActionResult GetByInvoiceItemId(ulong id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get all entities with the given car id
                    var carMaintenanceEntities = Context.CarMaintenances
                        .Where(cm => cm.InvoiceItemId.HasValue && cm.InvoiceItemId.Value == id).ToImmutableList();

                    if (carMaintenanceEntities.Count < 1)
                        return NoContent();

                    // Convert them to models and return OK
                    return Ok(CarMaintenanceAssembler.AssembleModelList(carMaintenanceEntities));
                }
                else
                    return BadRequest(new Error(301, GetModelStateErrorString(),
                        "The given id could not be converted. Please check the message for further information."));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }
    }
}
