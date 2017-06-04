using System.Collections.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ecruise.Models;
using ecruise.Models.Assemblers;
using GeoCoordinatePortable;
using DbCar = ecruise.Database.Models.Car;

namespace ecruise.Api.Controllers
{
    public class CarsController : BaseController
    {
        // GET: /cars
        [HttpGet(Name = "GetAllCars")]
        public IActionResult GetAll()
        {
            ImmutableList<DbCar> cars = Context.Cars.ToImmutableList();

            if (cars.Count == 0)
                return NoContent();

            return Ok(CarAssembler.AssembleModelList(cars));
        }


        // POST: /Cars
        [HttpPost(Name = "CreateCar")]
        public IActionResult Post([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCar insertCar = CarAssembler.AssembleEntity(0, car);

            var inserted = Context.Cars.Add(insertCar);
            Context.SaveChanges();

            return Created($"{BasePath}/cars/{inserted.Entity.CarId}",
                new PostReference((uint)inserted.Entity.CarId, $"{BasePath}/cars/{inserted.Entity.CarId}"));
        }

        // GET: /Cars/1
        [HttpGet("{id}", Name = "GetCar")]
        public IActionResult Get(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCar car = Context.Cars.Find(id);

            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));

            else
                return Ok(CarAssembler.AssembleModel(car));
        }

        // PATCH: /Cars/1/chargingState
        [HttpPatch("{id}/chargingState")]
        public IActionResult PatchChargingState(ulong id, [FromBody] Car.ChargingStateEnum chargingState)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCar car = Context.Cars.Find(id);

            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no car that has the id {id}."));

            car.ChargingState = CarAssembler.EnumToStringChargingState(chargingState);
            Context.SaveChanges();

            return Ok(new PostReference(id, $"{BasePath}/cars/{id}"));
        }

        // PATCH: /Cars/1/bookingState
        [HttpPatch("{id}/bookingState")]
        public IActionResult PatchBookingState(ulong id, [FromBody] Car.BookingStateEnum bookingState)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCar car = Context.Cars.Find(id);

            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no car that has the id {id}."));

            car.BookingState = CarAssembler.EnumToStringBookingState(bookingState);
            Context.SaveChanges();

            return Ok(new PostReference(id, $"{BasePath}/cars/{id}"));
        }

        // PATCH: /Cars/1/2
        [HttpPatch("{id}/{mileage}")]
        public IActionResult PatchMileage(ulong id, uint mileage)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Ok(new PostReference(id, $"{BasePath}/Cars/{id}"));
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "Car with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // PATCH: /Cars/1/2
        [HttpPatch("{id}/{chargelevel}")]
        public IActionResult PatchChargelevel(ulong id, uint chargelevel)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Ok(new PostReference(id, $"{BasePath}/Cars/{id}"));
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "Car with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // PATCH: /Cars/1/position/2.512/-5.215
        [HttpPatch("{id}/position/{latitude}/{longitude}")]
        public IActionResult PatchPosition(ulong id, double latitude, double longitude)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Ok(new PostReference(id, $"{BasePath}/Cars/{id}"));
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "Car with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /Cars/closest-to/58/8?radius=100
        // ReSharper disable PossibleInvalidOperationException
        [HttpGet(@"closest-to/{latitude}/{longitude}", Name = "GetClosestCar")]
        public IActionResult GetClosestCarChargingStation(double latitude, double longitude, [FromQuery]int radius)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            ImmutableList<DbCar> dbcars = Context.Cars
                .Where(c => c.LastKnownPositionLatitude != null && c.LastKnownPositionLongitude != null)
                .ToImmutableList();

            // render cars ordered by distance
            // if radius == 0: get only closest car
            // else if radius > 0: get all cars within radius
            GeoCoordinate destination = new GeoCoordinate(latitude, longitude);
            ImmutableList<DbCar> closest =
                dbcars
                    .Where(c => radius == 0 ||
                                destination.GetDistanceTo(new GeoCoordinate(c.LastKnownPositionLatitude.Value,
                                    c.LastKnownPositionLongitude.Value)) <= radius)
                    .OrderBy(c => destination.GetDistanceTo(new GeoCoordinate(c.LastKnownPositionLatitude.Value,
                        c.LastKnownPositionLongitude.Value)))
                    .ToImmutableList();

            // check if there are any matching cars
            if (dbcars.Count == 0)
                return NoContent();

            // if radius is zero (or not set) get only first element
            if (radius == 0)
                return Ok(CarAssembler.AssembleModelList(new List<DbCar> {closest.FirstOrDefault()}));
            else
                return Ok(CarAssembler.AssembleModelList(closest));
        }
    }
}
