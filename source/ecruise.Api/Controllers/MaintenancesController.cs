using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
using ecruise.Models.Assemblers;
using DbMaintenance = ecruise.Database.Models.Maintenance;

namespace ecruise.Api.Controllers
{
    public class MaintenancesController : BaseController
    {
        // GET: /Maintenances
        [HttpGet(Name = "GetAllMaintenances")]
        public IActionResult Get()
        {
            ImmutableList<DbMaintenance> maintenances = Context.Maintenances.ToImmutableList();

            if (maintenances.Count == 0)
                return NoContent();

            return Ok(MaintenanceAssembler.AssembleModelList(maintenances));
        }

        // GET: /Maintenances/5
        [HttpGet("{id}", Name = "GetMaintenance")]
        public IActionResult Get(uint id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbMaintenance maintenance = Context.Maintenances.Find(id);

            if (maintenance == null)
                return NotFound(new Error(201, "Maintenance with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));
            else
                return Ok(MaintenanceAssembler.AssembleModel(maintenance));
        }

        // POST: /Maintenances
        [HttpPost(Name = "CreateMaintenance")]
        public IActionResult Post([FromBody] Maintenance m)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbMaintenance insertMaintenance = MaintenanceAssembler.AssembleEntity(0, m);

            var insert = Context.Maintenances.Add(insertMaintenance);
            Context.SaveChanges();

            return Created($"{BasePath}/maintenances/{insert.Entity.MaintenanceId}",
                new PostReference((uint)insert.Entity.MaintenanceId, $"{BasePath}/maintenances/{insert.Entity.MaintenanceId}"));
        }
    }
}
