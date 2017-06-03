using System.Collections.Immutable;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
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
            return Ok(cars);
        }


        // POST: /Cars
        [HttpPost(Name = "CreateCar")]
        public IActionResult Post([FromBody] Car car)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCar insertCar = new DbCar
            {
                CarId = car.CarId,
                LicensePlate = car.LicensePlate,
                ChargingState = (ecruise.Database.Models.ChargingState) car.ChargingState,
                BookingState = (ecruise.Database.Models.BookingState) car.BookingState,
                Milage = car.Mileage,
                ChargeLevel = car.ChargeLevel,
                Kilowatts = car.Kilowatts,
                Manufacturer = car.Manufacturer,
                Model = car.Model,
                YearOfConstruction = car.YearOfConstruction,
                LastKnownPositionLatitude = car.LastKnownPositionLatitude,
                LastKnownPositionLongitude = car.LastKnownPositionLongitude,
                LastKnownPositionDate = car.LastKnownPositionDate,
            };

            var inserted = Context.Cars.Add(insertCar);

            return Created($"{BasePath}/cars/{inserted.Entity.CarId}",
                new PostReference((uint)inserted.Entity.CarId, $"{BasePath}/cars/{inserted.Entity.CarId}"));
        }

        // GET: /Cars/1
        [HttpGet("{id}", Name = "GetCar")]
        public IActionResult Get(uint id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCar car = Context.Cars.Find(id);

            if (car == null)
                return NotFound(new Error(201, "Car with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));
            else
                return Ok(car);
        }

        // PATCH: /Cars/1/2
        [HttpPatch("{id}/{chargingState}")]
        public IActionResult PatchChargingState(uint id, [FromBody] Car.ChargingStateEnum chargingState)
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
        [HttpPatch("{id}/{bookingState}")]
        public IActionResult PatchBookingState(uint id, [FromBody] Car.BookingStateEnum bookingState)
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
        [HttpPatch("{id}/{mileage}")]
        public IActionResult PatchMileage(uint id, uint mileage)
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
        public IActionResult PatchChargelevel(uint id, uint chargelevel)
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
        public IActionResult PatchPosition(uint id, double latitude, double longitude)
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

        // GET: /Cars/closest-to/58/8
        [HttpGet("closest-to/{latitude}/{longitude}", Name = "GetClosestCar")]
        public IActionResult GetClosestCarChargingStation(uint latitude, uint longitude)
        {
            if (ModelState.IsValid && latitude <= 90 && longitude <= 90)
            {
                Car car1 = new Car(1, "OG XY 123", Car.ChargingStateEnum.Full, Car.BookingStateEnum.Available, 1, 2.0, 100, "Audi", "A6", 2004, 48.5, 8.5, new DateTime(2017, 5, 8, 21, 5, 46));
                return Ok(car1);
            }
            else if (ModelState.IsValid && (latitude > 90 || latitude > 90))
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