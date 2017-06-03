using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ecruise.Models;
using ecruise.Models.Assemblers;
using GeoCoordinatePortable;
using DbChargingStation = ecruise.Database.Models.ChargingStation;

namespace ecruise.Api.Controllers
{
    [Route("v1/charging-stations")]
    public class ChargingStationsController : BaseController
    {
        // GET: /ChargingStations
        [HttpGet(Name = "GetAllChargingStations")]
        public IActionResult GetAll()
        {
            ImmutableList<DbChargingStation> chargingStations = Context.ChargingStations.ToImmutableList();

            if (chargingStations.Count == 0)
                return NoContent();

            return Ok(ChargingStationAssembler.AssembleModelList(chargingStations));
        }

        // POST: /ChargingStations
        [HttpPost(Name = "CreateChargingStation")]
        public IActionResult Post([FromBody] ChargingStation chargingStation)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbChargingStation insertChargingStation = ChargingStationAssembler.AssembleEntity(0, chargingStation);

            var inserted = Context.ChargingStations.Add(insertChargingStation);
            Context.SaveChanges();

            return Created($"{BasePath}/ChargingStations/{inserted.Entity.ChargingStationId}",
                new PostReference((uint)inserted.Entity.ChargingStationId,
                    $"{BasePath}/ChargingStations/{inserted.Entity.ChargingStationId}"));
        }

        // GET: /ChargingStations/5
        [HttpGet("{id}", Name = "GetChargingStation")]
        public IActionResult Get(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbChargingStation chargingStation = Context.ChargingStations.Find(id);

            if (chargingStation == null)
                return NotFound(new Error(201, "ChargingStation with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));
            else
                return Ok(chargingStation);
        }

        // GET: /ChargingStations/closest-to/58/8
        [HttpGet("closest-to/{latitude}/{longitude}", Name = "GetClosestChargingStation")]
        public IActionResult GetClosestChargingStation(double latitude, double longitude)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            ImmutableList<DbChargingStation> dbcs = Context.ChargingStations.ToImmutableList();

            // check if there are any charging stations
            if (dbcs.Count == 0)
                return NoContent();

            GeoCoordinate destination = new GeoCoordinate(latitude, longitude);
            DbChargingStation closest =
                dbcs.OrderBy(cs => destination.GetDistanceTo(new GeoCoordinate(cs.Latitude, cs.Longitude)))
                    .FirstOrDefault();

            return Ok(ChargingStationAssembler.AssembleModel(closest));
        }
    }
}
