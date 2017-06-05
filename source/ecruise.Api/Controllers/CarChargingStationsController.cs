using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecruise.Api.Controllers
{
    [Route("v1/car-charging-stations")]
    public class CarChargingStationsController : BaseController
    {
        // GET: /car-charging-stations
        [HttpGet(Name = "GetAllCarChargingStations")]
        public async Task<IActionResult> GetAllCharChargingStations()
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            try
            {
                // Get all entities from database
                var carChargingStationEntities = await Context.CarChargingStations.ToListAsync();

                if (carChargingStationEntities.Count < 1)
                    // Return that there are no results
                    return NoContent();

                // Return found car chargingstations
                return Ok(CarChargingStationAssembler.AssembleModelList(carChargingStationEntities));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // GET: /car-charging-stations/by-car/5
        [HttpGet("by-car/{carId}", Name = "GetCarChargingStationsByCar")]
        public async Task<IActionResult> GetByCarId(ulong carId)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            try
            {
                // Check for correct value
                if (!ModelState.IsValid)
                    return BadRequest(new Error(301,
                        "The id given was not formatted correctly. Id must be unsigned int",
                        "An error occured. Please check the message for further information."));

                // Get matching car chargingstation entities from database
                var carChargingStations = await Context.CarChargingStations.Where(ccs => ccs.CarId == carId)
                    .ToListAsync();

                if (carChargingStations == null || carChargingStations.Count < 1)
                    return NoContent();

                return Ok(CarChargingStationAssembler.AssembleModelList(carChargingStations));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // GET: /car-charging-stations/by-charging-station/5
        [HttpGet("by-charging-station/{chargingStationId}", Name = "GetCarChargingStationsByChargingStation")]
        public async Task<IActionResult> GetByChargingStationId(ulong chargingStationId)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            try
            {
                // Check for correct value
                if (!ModelState.IsValid)
                    return BadRequest(new Error(301,
                        "The id given was not formatted correctly. Id must be unsigned int",
                        "An error occured. Please check the message for further information."));

                // Get matching car chargingstation entities from database
                var carChargingStations = await Context.CarChargingStations
                    .Where(ccs => ccs.ChargingStationId == chargingStationId).ToListAsync();

                if (carChargingStations == null || carChargingStations.Count < 1)
                    return NoContent();

                return Ok(CarChargingStationAssembler.AssembleModelList(carChargingStations));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // POST: /car-charging-stations
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CarChargingStation carChargingStation)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            try
            {
                if (ModelState.IsValid)
                {
                    // Check object for logical validity
                    // Check car
                    if (Context.Customers.Find((ulong)carChargingStation.CarId) == null)
                        return NotFound(new Error(202, "The car referenced in the given object does not exist.",
                            "The referenced car must already exist to create a new car chargingstation."));

                    // Check charging station
                    if (Context.Customers.Find((ulong)carChargingStation.ChargingStationId) == null)
                        return NotFound(new Error(202,
                            "The chargingstation referenced in the given object does not exist.",
                            "The referenced chargingstation must already exist to create a new car chargingstation."));

                    // Check the charge start to be in the past
                    if (carChargingStation.ChargeStart.ToUniversalTime() > DateTime.UtcNow.AddMinutes(3))
                        return BadRequest(new Error(302, "Charge start date must be in the past",
                            "The charging must not start in the future."));

                    if (carChargingStation.ChargeEnd.HasValue)
                        return BadRequest(new Error(302, "Charge end date cannot be set.",
                            "The charge end date cannot already be set when creating the "));

                    // Construct entity from model
                    var carChargingStationEntity = CarChargingStationAssembler.AssembleEntity(0, carChargingStation);

                    // Save to database
                    await Context.CarChargingStations.AddAsync(carChargingStationEntity);
                    await Context.SaveChangesAsync();

                    // Get the reference to the newly created entity
                    PostReference pr = new PostReference((uint)carChargingStationEntity.CarChargingStationId,
                        $"{BasePath}/car-charging-stations/{carChargingStationEntity.CarChargingStationId}");

                    // Return reference to the new object including the path to it
                    return Created($"{BasePath}/car-charging-stations/{carChargingStationEntity.CarChargingStationId}",
                        pr);
                }

                return BadRequest(new Error(301, GetModelStateErrorString(),
                    "The given data could not be converted to a car chargingstation object. Please check the message for further information."));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // Patch: /car-charging-stations/5/charge-end
        [HttpPatch("{id}/charge-end")]
        public async Task<IActionResult> Patch(ulong id, [FromBody] string chargeEnd)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            // Transform string to date
            DateTime newChargeEndDateTime;
            if (DateTime.TryParseExact(chargeEnd, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out newChargeEndDateTime))
            {
                // Check given date for logical validity
                if (newChargeEndDateTime.ToUniversalTime() > DateTime.UtcNow)
                    return BadRequest(new Error(302, "Charge end date must be in the past.",
                        "The given date wasn't set properly. Please check the message for further information."));

                // Get the specified car chargingstation
                var carChargingStation = Context.CarChargingStations.Find(id);

                if (carChargingStation == null)
                    return NotFound(new Error(201, "A car chargingstation with requested id does not exist.",
                        "An error occured. Please check the message for further information."));

                // Patch completed date and save the change
                carChargingStation.ChargeEnd = newChargeEndDateTime;
                await Context.SaveChangesAsync();

                // Return a reference to the patch object
                return Ok(new PostReference(id, $"{BasePath}/car-charging-stations/{id}"));
            }

            return BadRequest(new Error(301, "The date given was not formatted correctly.",
                "Date must always be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'"));
        }
    }
}
