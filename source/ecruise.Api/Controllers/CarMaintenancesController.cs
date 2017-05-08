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
        [HttpGet]
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
                return Created($"{BasePath}CarMaintenances", new PostReference(1, "/CarMaintenances"));
            }
            else
            {
                return BadRequest(new Error(1, ModelState.ToString(),
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: api/CarMaintenances/5
        [HttpGet("{id}", Name = "Get")]
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
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        
        // PATCH: api/CarMaintenances/5
        [HttpPatch("{id}/completed-date/{date}")]
        public IActionResult Patch(uint id, string date)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                DateTime plannedDate;

                if (DateTime.TryParseExact(date, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
                    out plannedDate))
                {
                    CarMaintenance cm = new CarMaintenance(2, 1, 2, null, plannedDate, null);

                    return Ok(cm);
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
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsinged int",
                    "An error occured. Please check the message for further information."));
            }

        }
    }
}
