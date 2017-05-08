using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ecruise.Models;

namespace ecruise.Api.Controllers
{
    [Route("/CarChargingStations")]
    public class CarChargingStationsController : BaseController
    {
        // GET: /CarChargingStations
        [HttpGet]
        public IActionResult Get()
        {
            DateTime date = new DateTime(2017, 5, 10, 13, 37, 0, DateTimeKind.Utc);
            DateTime dateEnd = new DateTime(2017, 5, 10, 16, 37, 0, DateTimeKind.Utc);

            CarChargingStation ccs = new CarChargingStation(1, 1, 1, date, dateEnd);
            CarChargingStation ccs2 = new CarChargingStation(1, 1, 1, date.AddHours(2), null);

            return Ok(new List<CarChargingStation> { ccs, ccs2 });
        }

        // POST: /CarChargingStations
        [HttpPost]
        public IActionResult Post([FromBody]CarChargingStation carChargingStation)
        {
            if (ModelState.IsValid)
            {
                return Created($"{BasePath}/CarChargingStations", new PostReference(carChargingStation.CarChargingStationId, $"{BasePath}/CarChargingStations"));
            }
            else
            {
                return StatusCode(409);
            }
        }

        // Patch: /CarChargingStations/5
        [HttpPatch("{id}")]
        public IActionResult Patch(uint id, [FromBody]string chargeEnd)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                DateTime chargeEndDate;
                if (DateTime.TryParseExact(chargeEnd, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
                    out chargeEndDate))
                {
                    CarChargingStation ccs = new CarChargingStation(1, 1, 1, chargeEndDate.AddHours(-2), chargeEndDate);

                    return Ok(new PostReference(ccs.CarChargingStationId, $"{BasePath}/CarChargingStations"));
                }
                else
                {
                    return BadRequest(new Error(1, "The date given was not formatted correctly. Date must be in following format: 'dd.MM.yyyyTHH:mm:ss.zzzzZ'",
                        "An error occured. Please check the message for further information."));
                }
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "CarChargingStation with requested id does not exist.", "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /CarChargingStations/by-car/5
        [HttpGet("{carId}", Name = "GetCarChargingStationsByCar")]
        public IActionResult GetByCarId(uint carId)
        {
            if (ModelState.IsValid && carId < 3 && carId > 0)
            {
                DateTime date = new DateTime(2017, 5, 10, 13, 37, 0, DateTimeKind.Utc);
                DateTime dateEnd = new DateTime(2017, 5, 10, 16, 37, 0, DateTimeKind.Utc);

                CarChargingStation ccs = new CarChargingStation(1, carId, 1, date, dateEnd);
                CarChargingStation ccs2 = new CarChargingStation(1, carId, 1, date.AddHours(2), null);

                return Ok(new List<CarChargingStation> {ccs, ccs2});
            }
            else if (ModelState.IsValid && carId > 3)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /CarChargingStations/by-charging-station/5
        [HttpGet("{chargingStationId}", Name = "GetCarChargingStationsByChargingStation")]
        public IActionResult GetByChargingStationId(uint chargingStationId)
        {
            if (ModelState.IsValid && chargingStationId < 3 && chargingStationId > 0)
            {
                DateTime date = new DateTime(2017, 5, 10, 13, 37, 0, DateTimeKind.Utc);
                DateTime dateEnd = new DateTime(2017, 5, 10, 16, 37, 0, DateTimeKind.Utc);

                CarChargingStation ccs = new CarChargingStation(1, chargingStationId, 1, date, dateEnd);
                CarChargingStation ccs2 = new CarChargingStation(1, chargingStationId, 1, date.AddHours(2), null);

                return Ok(new List<CarChargingStation> { ccs, ccs2 });
            }
            else if (ModelState.IsValid && chargingStationId > 3)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));
            }
        }
    }
}
