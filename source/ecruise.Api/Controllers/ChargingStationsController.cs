using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ecruise.Models;

namespace ecruise.Api.Controllers
{
    [Route("v1/charging-stations")]
    public class ChargingStationsController : BaseController
    {
        // GET: /ChargingStations
        [HttpGet(Name = "GetAllChargingStations")]
        public IActionResult GetAll()
        {
            ChargingStation s1 = new ChargingStation(1, 2, 0, 49.485636, 8.4680978);
            ChargingStation s2 = new ChargingStation(2, 1, 1, 49.487877, 8.4704328);
            ChargingStation s3 = new ChargingStation(3, 3, 2, 49.487825, 8.4705938);
            
            return Ok(new List<ChargingStation> { s1, s2, s3 });
        }

        // POST: /ChargingStations
        [HttpPost(Name = "CreateChargingStation")]
        public IActionResult Post([FromBody]ChargingStation chargingStation)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/ChargingStations/1",
                    new PostReference(chargingStation.ChargingStationId, $"{BasePath}/ChargingStations/1"));
            else
                return BadRequest(new Error(1, ModelState.ToString(),
                    "An error occured. Please check the message for further information."));
        }

        // GET: /ChargingStations/5
        [HttpGet("{id}", Name = "GetChargingStation")]
        public IActionResult Get(uint id)
        {
            if (ModelState.IsValid && id < 3)
            {
                ChargingStation station1 = new ChargingStation(id, 2, 0, 49.485636, 8.4680978);
                return Ok(station1);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound(new Error(1, "ChargingStation with requested charging station id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /ChargingStations/closest-to/58/8
        [HttpGet("closest-to/{Latitude}/{Longitude}", Name = "GetClosestChargingStation")]
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
