using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ecruise.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecruise.Api.Controllers
{    
    [Route("Car-Maintenances")]
    public class CarMaintenancesController : BaseController
    {
        // GET: CarMaintenances
        [HttpGet(Name = "GetCarMaintenances")]
        public IActionResult Get()
        {
            CarMaintenance cm1 = new CarMaintenance(1, 1, 1, null, null, null);

            DateTime plannedDate = new DateTime(2017, 6, 1, 0, 0, 0, DateTimeKind.Utc);
            CarMaintenance cm2 = new CarMaintenance(2, 1, 2, null, plannedDate, null);

            List<CarMaintenance> list = new List<CarMaintenance>();
            list.Add(cm1);
            list.Add(cm2);

            return Ok(list);
        }

        // POST: CarMaintenances
        [HttpPost]
        public IActionResult Post([FromBody]CarMaintenance carMaintenance)
        {
            if (ModelState.IsValid)
            {
                return Created($"{BasePath}/CarMaintenances", new PostReference(1, "/CarMaintenances"));
            }
            else
            {
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /CarMaintenances/5
        [HttpGet("{id}", Name = "GetCarMaintenance")]
        public IActionResult Get(uint id)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Ok(new CarMaintenance(1, 1, 1, null, null, null));
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

        
        // PATCH: /CarMaintenances/5/completed-date/<date>
        [HttpPatch("{id}/completed-date/{date}")]
        public IActionResult Patch(uint id, [FromBody] string date)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                DateTime plannedDate;

                if (DateTime.TryParseExact(date, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
                    out plannedDate))
                {
                    CarMaintenance cm = new CarMaintenance(2, 1, 2, null, plannedDate, null);

                    return Ok(new PostReference(cm.CarMaintenanceId, $"{BasePath}/CarMaintenances"));
                }
                else
                {
                    return BadRequest(new Error(1, "The date given was not formatted correctly. Date must be in following format: 'dd.MM.yyyyTHH:mm:ss.zzzzZ'",
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
        public IActionResult GetByCarId(uint carId)
        {
            if (ModelState.IsValid && carId < 3 && carId > 0)
            {
                CarMaintenance cm1 = new CarMaintenance(1, carId, 1, null, null, null);
                CarMaintenance cm2 = new CarMaintenance(2, carId, 1, null, null, null);

                return Ok(new List<CarMaintenance> { cm1, cm2 });
            }
            else if (ModelState.IsValid && carId >= 3 && carId < 6)
            {
                return NoContent();
            }
            else if (ModelState.IsValid && carId >= 6)
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
        public IActionResult GetByMaintenanceId(uint maintenanceId)
        {
            if (ModelState.IsValid && maintenanceId < 3 && maintenanceId > 0)
            {
                CarMaintenance cm1 = new CarMaintenance(1, 1, maintenanceId, null, null, null);
                CarMaintenance cm2 = new CarMaintenance(2, 1, maintenanceId, null, null, null);

                return Ok(new List<CarMaintenance> {cm1, cm2});
            }
            else if (ModelState.IsValid && maintenanceId >= 3 && maintenanceId < 6)
            {
                return NoContent();
            }
            else if (ModelState.IsValid && maintenanceId >= 6)
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
        [HttpGet("by-invoice-item/{invoiceitemid}", Name = "GetCarMaintenanceByInvoiceItem")]
        public IActionResult GetByInvoiceItemId(uint invoiceItemId)
        {
            if (ModelState.IsValid && invoiceItemId < 3 && invoiceItemId > 0)
            {
                return Ok(new CarMaintenance(1, 1, 1, invoiceItemId, null, null));
            }
            else if (ModelState.IsValid && invoiceItemId >= 3 && invoiceItemId < 6)
            {
                return NoContent();
            }
            else if (ModelState.IsValid && invoiceItemId >= 6)
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
