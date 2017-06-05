using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ecruise.Models;
using ecruise.Models.Assemblers;
using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using DbChargingStation = ecruise.Database.Models.ChargingStation;

namespace ecruise.Api.Controllers
{
    [Route("v1/charging-stations")]
    public class ChargingStationsController : BaseController
    {
        // GET: /ChargingStations
        [HttpGet(Name = "GetAllChargingStations")]
        public async Task<IActionResult> GetAll()
        {
            // create a list of all charging stations
            List<DbChargingStation> chargingStations = await Context.ChargingStations.ToListAsync();

            // return 203 No Content if there are no charging stations
            if (chargingStations.Count == 0)
                return NoContent();

            return Ok(ChargingStationAssembler.AssembleModelList(chargingStations));
        }

        // POST: /ChargingStations
        [HttpPost(Name = "CreateChargingStation")]
        public async Task<IActionResult> Post([FromBody] ChargingStation chargingStation)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // create db charging station to be inserted
            DbChargingStation insertChargingStation = ChargingStationAssembler.AssembleEntity(0, chargingStation);

            // insert charging station into db
            var inserted = await Context.ChargingStations.AddAsync(insertChargingStation);

            await Context.SaveChangesAsync();

            return Created($"{BasePath}/ChargingStations/{inserted.Entity.ChargingStationId}",
                new PostReference((uint)inserted.Entity.ChargingStationId,
                    $"{BasePath}/ChargingStations/{inserted.Entity.ChargingStationId}"));
        }

        // GET: /ChargingStations/5
        [HttpGet("{id}", Name = "GetChargingStation")]
        public async Task<IActionResult> Get(ulong id)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested charging station
            DbChargingStation chargingStation = await Context.ChargingStations.FindAsync(id);

            // return error if charging station was not found
            if (chargingStation == null)
                return NotFound(new Error(201, "ChargingStation with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));

            return Ok(chargingStation);
        }

        // GET: /ChargingStations/closest-to/58/8
        [HttpGet("closest-to/{latitude}/{longitude}", Name = "GetClosestChargingStation")]
        public async Task<IActionResult> GetClosestChargingStation(double latitude, double longitude)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // get a list of all charging stations
            List<DbChargingStation> dbcs = await Context.ChargingStations.ToListAsync();

            // check if there are any charging stations
            if (dbcs.Count == 0)
                return NoContent();

            // create a location object of the requested location
            GeoCoordinate destination = new GeoCoordinate(latitude, longitude);

            // find the closest charging station
            DbChargingStation closest =
                dbcs.OrderBy(cs => destination.GetDistanceTo(new GeoCoordinate(cs.Latitude, cs.Longitude)))
                    .FirstOrDefault();

            return Ok(ChargingStationAssembler.AssembleModel(closest));
        }
    }
}
