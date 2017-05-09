using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ecruise.Models;

namespace ecruise.Api.Controllers
{
    [Route("/Cars")]
    public class CarsController : BaseController
    {
        // GET: /cars
        [HttpGet(Name = "GetAllCars")]
        public IActionResult GetAll()
        {
            Car car1 = new Car(1, "OG-XY-123", Car.ChargingStateEnum.Full, Car.BookingStateEnum.Available, 1, 2.0, 100, "Audi", "A6", 2004, 48.5, 8.5, new DateTime(2017, 5, 8, 21, 5, 46));
            Car car2 = new Car(1, "OG-XY-123", Car.ChargingStateEnum.Full, Car.BookingStateEnum.Available, 1, 7.9, 100, "Audi", "A6", 2004, 17.23, 84.6, new DateTime(2017, 5, 8, 21, 5, 46));
            
            return Ok(new List<Car> { car1, car2});
        }


        // POST: /Cars
        [HttpPost(Name = "CreateCar")]
        public IActionResult Post([FromBody] Car car)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/cars/1",
                    new PostReference(car.CarId, $"{BasePath}/cars/1"));
            else
                return BadRequest(new Error(1, ModelState.ToString(),
                    "An error occured. Please check the message for further information."));
        }

        // GET: /Cars/1
        [HttpGet("{id}", Name = "GetCar")]
        public IActionResult Get(uint id)
        {
            if (ModelState.IsValid && id < 3)
            {
                Car car1 = new Car(1, "OG-XY-123", Car.ChargingStateEnum.Full, Car.BookingStateEnum.Available, 1, 2.0, 100, "Audi", "A6", 2004, 48.5, 8.5, new DateTime(2017, 5, 8, 21, 5, 46));

                return Ok(car1);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound(new Error(1, "Car with requested Car id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // PATCH: /Cars/1/2
        [HttpPatch("{id}/{chargingState}")]
        public IActionResult PatchChargingState(uint id, [FromBody] Car.ChargingStateEnum chargingState)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Created($"{BasePath}/Cars/{id}",
                    new PostReference(id, $"{BasePath}/Cars/{id}"));
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
                return Created($"{BasePath}/Cars/{id}",
                    new PostReference(id, $"{BasePath}/Cars/{id}"));
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
                return Created($"{BasePath}/Cars/{id}",
                    new PostReference(id, $"{BasePath}/Cars/{id}"));
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
                return Created($"{BasePath}/Cars/{id}",
                    new PostReference(id, $"{BasePath}/Cars/{id}"));
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
        [HttpPatch("{id}/{position}")]
        public IActionResult PatchPosition(uint id, [FromBody]  string position)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Created($"{BasePath}/Cars/{id}",
                    new PostReference(id, $"{BasePath}/Cars/{id}"));
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
        [HttpGet("closest-to/{Latitude}/{Longitude}", Name = "GetClosestCar")]
        public IActionResult GetClosestCarChargingStation(uint Latitude, uint Longitude)
        {
            if (ModelState.IsValid && Latitude <= 90 && Longitude <= 90)
            {
                Car car1 = new Car(1, "OG-XY-123", Car.ChargingStateEnum.Full, Car.BookingStateEnum.Available, 1, 2.0, 100, "Audi", "A6", 2004, 48.5, 8.5, new DateTime(2017, 5, 8, 21, 5, 46));
                return Ok(car1);
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