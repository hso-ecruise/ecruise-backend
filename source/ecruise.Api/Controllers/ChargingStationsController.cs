using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChargingStation = ecruise.Models.ChargingStation;
using DbChargingStation = ecruise.Database.Models.ChargingStation;

namespace ecruise.Api.Controllers
{
    [Route("v1/charging-stations")]
    public class ChargingStationsController : BaseController
    {
        public ChargingStationsController(EcruiseContext context) : base(context)
        {
        }

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
                return Unauthorized();

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
                    $"There is no charging station that has the id {id}."));

            return Ok(chargingStation);
        }

        // GET: /ChargingStations/5/decrement-slots-occupied
        [HttpPatch("{id}/decrement-slots-occupied")]
        public async Task<IActionResult> DecrementSlotsOccupied(ulong id)
        {
            // Forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // Get the charging station from the database
            var chargingStation = await Context.ChargingStations.FindAsync(id);

            // Check if charging station was found
            if (chargingStation == null)
                return NotFound(new Error(201, "ChargingStation with requested id does not exist.",
                    $"There is no chargin station that has the id {id}."));

            // Check the slots occupied
            if(chargingStation.Slots == 0)
                return BadRequest(new Error(302, "SlotsOccupied is already zero",
                    "The occupied slots could not be decremented because there are already zero slots occupied"));

            // Change the value
            chargingStation.SlotsOccupied--;

            // Save the change to the database
            await Context.SaveChangesAsync();

            return Ok(new PostReference(chargingStation.ChargingStationId,
                    $"{BasePath}/ChargingStations/{chargingStation.ChargingStationId}"));
        }

        // GET: /ChargingStations/closest-to/58/8
        [HttpGet("closest-to/{latitude}/{longitude}", Name = "GetClosestChargingStation")]
        public async Task<IActionResult> GetClosestChargingStation(double latitude, double longitude, [FromQuery] uint minFreeSlots, [FromQuery] uint radius)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // get a list of all charging stations
            List<DbChargingStation> chargingStations = await Context.ChargingStations.ToListAsync();

            // check if there are any charging stations
            if (chargingStations.Count == 0)
                return NoContent();

            // only return closest if radius equals 0
            if (radius == 0)
            {
                DbChargingStation closest = null;

                // Find first matching entry
                if (minFreeSlots == 0)
                {
                    // Dont filter. Only return closest
                    closest = chargingStations
                        .FirstOrDefault();
                }
                else // minFreeSlots != 0
                {
                    // Filter by free slots
                    closest = chargingStations
                        .FirstOrDefault(cs => cs.Slots - cs.SlotsOccupied >= minFreeSlots);
                }

                // Check if any found
                if (closest == null)
                    return NoContent();

                // Return found entry
                return Ok(ChargingStationAssembler.AssembleModel(closest));
            }

            // create a location object of the requested location
            GeoCoordinate destination = new GeoCoordinate(latitude, longitude);

            // Get only entries in the given radius
            var chargingStationsInRadius = chargingStations.Where(
                cs => destination.GetDistanceTo(new GeoCoordinate(cs.Latitude, cs.Longitude)) <= radius).ToList();

            // If filter by min free slots is set
            if (minFreeSlots > 0)
            {
                // Remove charging stations with less than the desired count
                chargingStationsInRadius.RemoveAll(cs => cs.Slots - cs.SlotsOccupied < minFreeSlots);
            }

            // Check if any left
            if (chargingStationsInRadius.Count == 0)
                return NoContent();

            // Return matching entries
            return Ok(ChargingStationAssembler.AssembleModelList(chargingStationsInRadius));
        }
    }
}
