using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.EntityFrameworkCore;
using DbMaintenance = ecruise.Database.Models.Maintenance;

namespace ecruise.Api.Controllers
{
    public class MaintenancesController : BaseController
    {
        // GET: /Maintenances
        [HttpGet(Name = "GetAllMaintenances")]
        public async Task<IActionResult> Get()
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            List<DbMaintenance> maintenances = await Context.Maintenances.ToListAsync();

            if (maintenances.Count == 0)
                return NoContent();

            return Ok(MaintenanceAssembler.AssembleModelList(maintenances));
        }

        // GET: /Maintenances/5
        [HttpGet("{id}", Name = "GetMaintenance")]
        public async Task<IActionResult> Get(ulong id)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbMaintenance maintenance = await Context.Maintenances.FindAsync(id);

            if (maintenance == null)
                return NotFound(new Error(201, "Maintenance with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));
            else
                return Ok(MaintenanceAssembler.AssembleModel(maintenance));
        }

        // POST: /Maintenances
        [HttpPost(Name = "CreateMaintenance")]
        public async Task<IActionResult> Post([FromBody] Maintenance m)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbMaintenance insertMaintenance = MaintenanceAssembler.AssembleEntity(0, m);

            var insert = await Context.Maintenances.AddAsync(insertMaintenance);
            await Context.SaveChangesAsync();

            return Created($"{BasePath}/maintenances/{insert.Entity.MaintenanceId}",
                new PostReference((uint)insert.Entity.MaintenanceId, $"{BasePath}/maintenances/{insert.Entity.MaintenanceId}"));
        }
    }
}
