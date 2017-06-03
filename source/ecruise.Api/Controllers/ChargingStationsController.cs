using System.Collections.Immutable;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
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
            return Ok(chargingStations);
        }

        // POST: /ChargingStations
        [HttpPost(Name = "CreateChargingStation")]
        public IActionResult Post([FromBody]ChargingStation chargingStation)
        { 
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbChargingStation insertChargingStation = new DbChargingStation
            {
                ChargingStationId = chargingStation.ChargingStationId,
                Slots = chargingStation.Slots,
                SlotsOccupied = chargingStation.SlotsOccupuied,
                Latitude = chargingStation.Latitude,
                Longitude = chargingStation.Longitude,
            };

        var inserted = Context.ChargingStations.Add(insertChargingStation);

            return Created($"{BasePath}/ChargingStations/{inserted.Entity.ChargingStationId}",
                new PostReference((uint) inserted.Entity.ChargingStationId, $"{BasePath}/ChargingStations/{inserted.Entity.ChargingStationId}"));
        }

    // GET: /ChargingStations/5
    [HttpGet("{id}", Name = "GetChargingStation")]
        public IActionResult Get(uint id)
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
        public IActionResult GetClosestChargingStation(uint latitude, uint longitude)
        {
            if (ModelState.IsValid)
            {
                ChargingStation station1 = new ChargingStation(1, 2, 0, 49.485636, 8.4680978);
                return Ok(station1);
            }
            else if (ModelState.IsValid)
            {
                return NotFound(new Error(1, "Position does not exist on earth.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

    }
}
