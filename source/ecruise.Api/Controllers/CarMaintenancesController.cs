using System.Collections.Immutable;
using System.Globalization;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

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
                    return NotFound(new Error(201, "A car maintenance with requested booking id does not exist.",
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
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                DateTime plannedDate;

                if (DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture, 
                    DateTimeStyles.AssumeUniversal, out plannedDate))
                {
                    CarMaintenance cm = new CarMaintenance(2, 1, 2, null, plannedDate, null);

                    return Ok(new PostReference(cm.CarMaintenanceId, $"{BasePath}/CarMaintenances"));
                }
                else
                {
                    return BadRequest(new Error(1, "The date given was not formatted correctly. Date must be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'",
                        "An error occured. Please check the message for further information."));
                }
                 
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "CarMaintenance with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /CarMaintenances/by-car/5
        [HttpGet("by-car/{id}", Name = "GetCarMaintenancesByCar")]
        public IActionResult GetByCarId(ulong id)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                CarMaintenance cm1 = new CarMaintenance(1, 1, 1, null, null, null);
                CarMaintenance cm2 = new CarMaintenance(2, 1, 1, null, null, null);

                return Ok(new List<CarMaintenance> { cm1, cm2 });
            }
            else if (ModelState.IsValid && id >= 3 && id < 6)
            {
                return NoContent();
            }
            else if (ModelState.IsValid && id >= 6)
            {
                return NotFound(new Error(1, "CarMaintenance with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /CarMaintenances/by-maintenance/5
        [HttpGet("by-maintenance/{id}", Name = "GetCarMaintenancesByMaintenance")]
        public IActionResult GetByMaintenanceId(ulong id)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                CarMaintenance cm1 = new CarMaintenance(1, 1, 1, null, null, null);
                CarMaintenance cm2 = new CarMaintenance(2, 1, 1, null, null, null);

                return Ok(new List<CarMaintenance> {cm1, cm2});
            }
            else if (ModelState.IsValid && id >= 3 && id < 6)
            {
                return NoContent();
            }
            else if (ModelState.IsValid && id >= 6)
            {
                return NotFound(new Error(1, "Maintenance with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /CarMaintenances/by-invoice-item/5
        [HttpGet("by-invoice-item/{id}", Name = "GetCarMaintenanceByInvoiceItem")]
        public IActionResult GetByInvoiceItemId(ulong id)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Ok(new CarMaintenance(1, 1, 1, 1, null, null));
            }
            else if (ModelState.IsValid && id >= 3 && id < 6)
            {
                return NoContent();
            }
            else if (ModelState.IsValid && id >= 6)
            {
                return NotFound(new Error(1, "InvoiceItem with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));
            }
        }
    }
}
