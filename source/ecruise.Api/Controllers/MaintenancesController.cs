using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;

namespace ecruise.Api.Controllers
{
    public class MaintenancesController : BaseController
    {
        // GET: /Maintenances
        [HttpGet(Name="GetAllMaintenances")]
        public IActionResult Get()
        {
            CarMaintenance cm1 = new CarMaintenance(1, 3, 1, null, null, null);
            CarMaintenance cm2 = new CarMaintenance(2, 3, 1, null, new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc), null);
            CarMaintenance cm3 = new CarMaintenance(3, 3, 1, 6, null, new DateTime(2017, 5, 8, 13, 50, 0, DateTimeKind.Utc));
            CarMaintenance cm4 = new CarMaintenance(4, 3, 1, 6, new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc), new DateTime(2017, 5, 8, 13, 50, 0, DateTimeKind.Utc));

            return Ok(new List<CarMaintenance> {cm1, cm2, cm3, cm4});
        }

        // GET: /Maintenances/5
        [HttpGet("{id}", Name = "GetMaintenance")]
        public IActionResult Get(uint id)
        {
            if (ModelState.IsValid && id > 0 && id < 3)
            {
                CarMaintenance cm = new CarMaintenance(id, 3, 1, 6, new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc), new DateTime(2017, 5, 8, 13, 50, 0, DateTimeKind.Utc));

                return Ok(cm);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound(new Error(1, "Maintenance with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }
        
        // POST: /Maintenances
        [HttpPost(Name="CreateMaintenance")]
        public IActionResult Post([FromBody]CarMaintenance m)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/maintenances/1",
                    new PostReference(1, $"{BasePath}/maintenances/1"));
            else
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
        }
    }
}
