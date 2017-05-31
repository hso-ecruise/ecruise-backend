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
            Maintenance m1 = new Maintenance(1, false, 3000, null);
            Maintenance m2 = new Maintenance(2, false, null, new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc));
            Maintenance m3 = new Maintenance(3, true, null, null);

            return Ok(new List<Maintenance> {m1, m2, m3});
        }

        // GET: /Maintenances/5
        [HttpGet("{id}", Name = "GetMaintenance")]
        public IActionResult Get(uint id)
        {
            if (ModelState.IsValid && id > 0 && id < 3)
            {
                Maintenance m = new Maintenance(1, false, 3000, null);

                return Ok(m);
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
        public IActionResult Post([FromBody]Maintenance m)
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
