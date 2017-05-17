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
        [HttpPost(Name = "CreateCarChargingStation")]
        public IActionResult Post([FromBody]CarChargingStation station)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/ChargingStations/1",
                    new PostReference(station.CarChargingStationId, $"{BasePath}/ChargingStations/1"));
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
                CarChargingStation station1 = new CarChargingStation(1, 1, 1, new DateTime(2017, 5, 8, 21, 5, 46), new DateTime(2017, 5, 8, 21, 57, 23));
                return Ok(station1);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound(new Error(1, "CarChargingStation with requested CarChargingStation id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /ChargingStations/closest-to/58/8
        [HttpGet("closest-to/{Latitude}/{Longitude}", Name = "GetClosestCarChargingStation")]
        public IActionResult GetClosestCarChargingStation(uint Latitude, uint Longitude)
        {
            if (ModelState.IsValid && Latitude <= 90 && Longitude <= 90)
            {
                CarChargingStation station1 = new CarChargingStation(1, 1, 1, new DateTime(2017, 5, 8, 21, 5, 46), new DateTime(2017, 5, 8, 21, 57, 23));
                return Ok(station1);
            }
            else if (ModelState.IsValid && (Latitude > 90 || Latitude > 90))
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
