using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Car = ecruise.Models.Car;
using DbCar = ecruise.Database.Models.Car;

namespace ecruise.Api.Controllers
{
    public class CarsController : BaseController
    {
        public CarsController(EcruiseContext context) : base(context)
        {
        }

        // GET: /cars
        [HttpGet(Name = "GetAllCars")]
        public async Task<IActionResult> GetAll()
        {
            // create a list of all cars
            List<DbCar> cars = await Context.Cars.ToListAsync();

            // return 203 No Content if there are no cars
            if (cars.Count == 0)
                return NoContent();

            return Ok(CarAssembler.AssembleModelList(cars));
        }


        // POST: /Cars
        [HttpPost(Name = "CreateCar")]
        public async Task<IActionResult> Post([FromBody] Car car)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // create the db car to be inserted
            DbCar insertCar = CarAssembler.AssembleEntity(0, car);

            // insert db car into database
            var inserted = await Context.Cars.AddAsync(insertCar);

            await Context.SaveChangesAsync();

            return Created($"{BasePath}/cars/{inserted.Entity.CarId}",
                new PostReference((uint)inserted.Entity.CarId, $"{BasePath}/cars/{inserted.Entity.CarId}"));
        }

        // GET: /Cars/1
        [HttpGet("{id}", Name = "GetCar")]
        public async Task<IActionResult> Get(ulong id)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested car
            DbCar car = await Context.Cars.FindAsync(id);

            // return error if car was not found
            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));

            return Ok(CarAssembler.AssembleModel(car));
        }

        // GET: /Cars/charge-level-per-minute
        [HttpGet("charge-level-per-minute", Name = "GetChargeLevelPerMinute")]
        public IActionResult GetChargeLevelPerMinute()
        {
            return Ok(1.0);
        }

        // GET: /Cars/1/find
        [HttpGet("{id}/find", Name = "FindCar")]
        public async Task<IActionResult> FindCar(ulong id)
        {
            // Validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // Find the requested car
            DbCar car = await Context.Cars.FindAsync(id);

            // Return error if car was not found
            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no car that has the id {id}."));

            // Get the trips where the customer is allowed to seach for the car
            var trips = await Context.Trips.Where(t => t.CustomerId == AuthenticatedCustomerId && t.EndDate == null)
                .ToListAsync();

            if (trips.Count == 0)
            {
                return NotFound(new Error(201, "Customer has no trips.",
                    "There is no trip that belongs to this customer."));
            }

            if (trips.FirstOrDefault(t => t.CarId == id) == null)
            {
                return NotFound(new Error(201, "Car with requested id has no trips.",
                    "There is no trip that belongs to the car with the requested id."));
            }

            // Remember lastKnownPositionDate
            var lastKnownPositionDate = car.LastKnownPositionDate;

            // Write the car in the search list
            var config = await Context.Configurations.FindAsync((ulong)1);
            var searchedCarsString = config.SearchedCars;
            
            List<ulong> listOfSearchedCarIds = new List<ulong>();

            if(searchedCarsString != string.Empty)
                listOfSearchedCarIds = searchedCarsString.Split(',').Select(ulong.Parse).ToList();

            // Check if car not already in (e.g. by multiple same requests)
            // ReSharper disable once SimplifyLinqExpression
            if (!listOfSearchedCarIds.Any(carId => carId == id))
            {
                // Check if the string is empty (no comma needed than)
                if (searchedCarsString != string.Empty)
                    searchedCarsString += ",";

                // Add the car to search
                searchedCarsString += id.ToString();

                // Update the entity
                config.SearchedCars = searchedCarsString;

                // Save the changes
                await Context.SaveChangesAsync();
            }

            // Wait for car to set its location
            while (true)
            {
                // Wait 1 second
                Thread.Sleep(1000);

                // Check if the lastKnownPositionDate has changed
                // Get current car from db
                Context.Entry(car).Reload();

                // Check if the date has changed
                if (car.LastKnownPositionDate != lastKnownPositionDate)
                {
                    // The date has changed so the update must have been done
                    return Ok(CarAssembler.AssembleModel(car));
                }
            }
        }

        // GET: /Cars/1
        [HttpGet("{id}/is-wanted", Name = "CheckCarIsWanted")]
        public async Task<IActionResult> CheckCarIsWanted(ulong id)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested car
            DbCar car = await Context.Cars.FindAsync(id);

            // return error if car was not found
            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));

            // Get the configuration
            var config = await Context.Configurations.FindAsync((ulong) 1);

            var searchedCarsString = config.SearchedCars;

            // Check if any car is searched
            if (searchedCarsString == string.Empty)
                return NoContent();

            // Split the string and convert to ulongs
            var splittedSearchedCarIds = searchedCarsString.Split(',');
            var searchedCarIds = splittedSearchedCarIds.Select(ulong.Parse).ToList();

            // Check if the given car is searched
            if (searchedCarIds.Contains(id))
                return Ok(true);

            return NoContent();
        }

        // PATCH: /Cars/1/chargingState
        [HttpPatch("{id}/chargingState")]
        public async Task<IActionResult> PatchChargingState(ulong id, [FromBody] Car.ChargingStateEnum chargingState)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested car
            DbCar car = await Context.Cars.FindAsync(id);

            // return error if car was not found
            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no car that has the id {id}."));

            // update the car's charging state
            car.ChargingState = CarAssembler.EnumToStringChargingState(chargingState);

            await Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/cars/{id}"));
        }

        // PATCH: /Cars/1/bookingState
        [HttpPatch("{id}/bookingState")]
        public async Task<IActionResult> PatchBookingState(ulong id, [FromBody] Car.BookingStateEnum bookingState)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested car
            DbCar car = await Context.Cars.FindAsync(id);

            // return error if car was not found
            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no car that has the id {id}."));

            // update the car's booking state
            car.BookingState = CarAssembler.EnumToStringBookingState(bookingState);

            await Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/cars/{id}"));
        }

        // PATCH: /Cars/1/mileage
        [HttpPatch("{id}/mileage")]
        public async Task<IActionResult> PatchMileage(ulong id, [FromBody] uint mileage)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested car
            DbCar car = await Context.Cars.FindAsync(id);

            // return error if car was not found
            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no car that has the id {id}."));

            // update the car's mileage
            car.Milage = mileage;

            await Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/cars/{id}"));
        }

        // PATCH: /Cars/1/chargelevel
        [HttpPatch("{id}/chargelevel")]
        public async Task<IActionResult> PatchChargelevel(ulong id, [FromBody] double chargelevel)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested car
            DbCar car = await Context.Cars.FindAsync(id);

            // return error if car was not found
            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no car that has the id {id}."));

            // update the car's charge level
            car.ChargeLevel = chargelevel;

            await Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/cars/{id}"));
        }

        // PATCH: /Cars/1/position/2.512/-5.215
        [HttpPatch("{id}/position/{latitude}/{longitude}")]
        public async Task<IActionResult> PatchPosition(ulong id, double latitude, double longitude)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested car
            DbCar car = await Context.Cars.FindAsync(id);

            // return error if car was not found
            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no car that has the id {id}."));

            // update the car's last known position and the associated date
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                car.LastKnownPositionLatitude = latitude;
                car.LastKnownPositionLongitude = longitude;
                car.LastKnownPositionDate = DateTime.UtcNow;

                // TODO Felix: Check if car was searched

                transaction.Commit();
                await Context.SaveChangesAsync();
            }

            return Ok(new PostReference(id, $"{BasePath}/cars/{id}"));
        }

        // GET: /Cars/closest-to/58/8?radius=100
        // ReSharper disable PossibleInvalidOperationException
        [HttpGet(@"closest-to/{latitude}/{longitude}", Name = "GetClosestCar")]
        public async Task<IActionResult> GetClosestCarChargingStation(double latitude, double longitude,
            [FromQuery] int radius)
        {
            // Validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // Get a list of all cars with a known position
            List<DbCar> dbcars = await Context.Cars
                .Where(c => c.LastKnownPositionLatitude != null && c.LastKnownPositionLongitude != null)
                .ToListAsync();

            // Return 404 No Content if there are no matching cars
            if (dbcars.Count == 0)
                return NotFound(new Error(201, "There are no cars in the database.",
                    $"There is no cars in the database."));

            // Render cars ordered by distance
            // if radius == 0: get only closest car
            // else if radius > 0: get all cars within radius
            GeoCoordinate destination = new GeoCoordinate(latitude, longitude);
            ImmutableList<DbCar> closest =
                dbcars.Where(c => radius == 0 ||
                                  destination.GetDistanceTo(new GeoCoordinate(c.LastKnownPositionLatitude.Value,
                                      c.LastKnownPositionLongitude.Value)) <= radius)
                    .OrderBy(c => destination.GetDistanceTo(new GeoCoordinate(c.LastKnownPositionLatitude.Value,
                        c.LastKnownPositionLongitude.Value)))
                    .ToImmutableList();

            // Check if any found
            if (closest.Count == 0)
            {
                return NoContent();
            }

            // If radius is zero (or not set) get only first element
            if (radius == 0)
                return Ok(CarAssembler.AssembleModelList(new List<DbCar> { closest.FirstOrDefault() }));

            return Ok(CarAssembler.AssembleModelList(closest));
        }
    }
}
