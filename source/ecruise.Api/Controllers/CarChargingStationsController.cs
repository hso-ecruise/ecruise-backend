using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarChargingStation = ecruise.Models.CarChargingStation;

namespace ecruise.Api.Controllers
{
    [Route("v1/car-charging-stations")]
    public class CarChargingStationsController : BaseController
    {
        public CarChargingStationsController(EcruiseContext context) : base(context)
        {
        }

        // GET: /car-charging-stations
        [HttpGet(Name = "GetAllCarChargingStations")]
        public async Task<IActionResult> GetAllCarChargingStationsAsync()
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Get all entities from database
            var carChargingStationEntities = await Context.CarChargingStations.ToListAsync();

            if (carChargingStationEntities.Count < 1)
                // Return that there are no results
                return NoContent();

            // Return found car chargingstations
            return Ok(CarChargingStationAssembler.AssembleModelList(carChargingStationEntities));
        }

        // GET: /car-charging-stations/by-car/5
        [HttpGet("by-car/{carId}", Name = "GetCarChargingStationsByCar")]
        public async Task<IActionResult> GetByCarIdAsync(ulong carId)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

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


        // GET: /car-charging-stations/by-charging-station/5
        [HttpGet("by-charging-station/{chargingStationId}", Name = "GetCarChargingStationsByChargingStation")]
        public async Task<IActionResult> GetByChargingStationIdAsync(ulong chargingStationId)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

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

        // POST: /car-charging-stations
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CarChargingStation carChargingStation)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Check object for logical validity
            if (!ModelState.IsValid)
                return BadRequest(new Error(301, GetModelStateErrorString(),
                    "The given data could not be converted to a car chargingstation object. Please check the message for further information."
                ));

            // Check car
            var car = await Context.Cars.FindAsync((ulong)carChargingStation.CarId);
            if (car == null)
                return NotFound(new Error(202, "The car referenced in the given object does not exist.",
                    "The referenced car must already exist to create a new car chargingstation."));

            // Check charging station
            var chargingStation = await Context.ChargingStations.FindAsync((ulong)carChargingStation.ChargingStationId);
            if (chargingStation == null)
                return NotFound(new Error(202,
                    "The chargingstation referenced in the given object does not exist.",
                    "The referenced chargingstation must already exist to create a new car chargingstation."));

            // Check the charge start to be in the past
            if (carChargingStation.ChargeStart.ToUniversalTime() > DateTime.UtcNow.AddMinutes(3))
                return BadRequest(new Error(302, "Charge start date must be in the past",
                    "The charging must not start in the future."));

            if (carChargingStation.ChargeEnd.HasValue)
                return BadRequest(new Error(302, "Charge end date cannot be set.",
                    "The charge end date cannot already be set when creating the car charging station"));

            // Set the position because the car is connected now
            car.LastKnownPositionDate = DateTime.UtcNow;
            car.LastKnownPositionLatitude = chargingStation.Latitude;
            car.LastKnownPositionLongitude = chargingStation.Longitude;

            if (car.BookingState == "BOOKED")
                car.BookingState = "AVAILABLE";

            chargingStation.SlotsOccupied = chargingStation.SlotsOccupied == 0
                ? 0
                : chargingStation.SlotsOccupied - 1;

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

        // Patch: /car-charging-stations/5/charge-end
        [HttpPatch("{id}/charge-end")]
        public async Task<IActionResult> PatchAsync(ulong id, [FromBody] string chargeEnd)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Transform string to date
            DateTime newChargeEndDateTime;
            if (!DateTime.TryParseExact(chargeEnd, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out newChargeEndDateTime))
                return BadRequest(new Error(301, "The date given was not formatted correctly.",
                    "Date must always be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'"));

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
    }
}
