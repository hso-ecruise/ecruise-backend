using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecruise.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Bookings")]
    public class BookingsController : Controller
    {
        // GET: api/Bookings/5
        [HttpGet("{id}", Name = "GetBooking")]
        public IActionResult Get(uint id)
        {
            if (ModelState.IsValid && id < 3)
            {
                Booking booking = new Booking(id, 1, 1, 1, 49.488342, 8.466788, new DateTime(2017, 5, 8, 13, 37, 0),
                new DateTime(2017, 5, 10, 13, 37, 0));
                return Ok(booking);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound("Booking with requested booking id does not exist.");
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }
        
        // POST: api/Bookings
        [HttpPost(Name = "PostBooking")]
        public IActionResult Post([FromBody]Booking booking)
        {
            if (ModelState.IsValid)
                return Created($"api/Bookings/1",
                    new PostReference(booking.BookingId, "api/Bookings/"));
            else
                return BadRequest(new Error(1, ModelState.ToString(),   //TODO ModelState.ToString ok for information?
                    "An error occured. Please check the message for further information."));
        }
        
        // GET: api/Bookings/by-trip/5
        [Route("/by-trip")]
        [HttpGet("{tripid}", Name = "GetBookingByTrip")]
        public IActionResult GetByTripId(uint tripid)
        {
            if (ModelState.IsValid && tripid < 3)
            {
                Booking booking = new Booking(1, 1, tripid, 1, 49.488342, 8.466788, new DateTime(2017, 5, 8, 13, 37, 0),
                    new DateTime(2017, 5, 10, 13, 37, 0));
                return Ok(booking);
            }
            else if (ModelState.IsValid && (tripid >= 3 || tripid == 0))
            {
                return NotFound("Booking with requested trip id does not exist.");
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. TripId must be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: api/Bookings/by-customer/5
        [Route("/by-customer")]
        [HttpGet("{customerid}", Name = "GetBookingByCustomer")]
        public IActionResult GetByCustomerId(uint customerid)
        {
            if (ModelState.IsValid && customerid < 3 && customerid > 0)
            {
                Booking booking = new Booking(1, customerid, 1, 1, 49.488342, 8.466788, new DateTime(2017, 5, 8, 13, 37, 0),
                    new DateTime(2017, 5, 10, 13, 37, 0));
                return Ok(booking);
            }
            else if (ModelState.IsValid && customerid >= 3)
            {
                return NotFound("Booking with requested customer id does not exist.");
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. CustomerId must be unsinged int greater than zero",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: api/Bookings/by-trip/5
        [HttpGet("{date}", Name = "GetBookingsByDate")]
        public IActionResult GetByDate(string date)
        {
            // Transform string to date
            DateTime requestedDateTime;
            if (DateTime.TryParseExact(date, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out requestedDateTime))
            {
                Booking booking1 = new Booking(1, 1, 1, 1, 49.488342, 8.466788, new DateTime(2017, 5, 8, 13, 37, 0),
                    requestedDateTime);
                Booking booking2 = new Booking(2, 1, 1, 1, 49.488342, 8.466788, new DateTime(2017, 5, 8, 13, 37, 0),
                    requestedDateTime);

                List<Booking> list = new List<Booking>();
                list.Add(booking1);
                list.Add(booking2);

                return Ok(list);
            }
            else
            {
                return BadRequest(new Error(1, "The date given was not formatted correctly. Date must be in following format: 'dd.MM.yyyyTHH:mm:ss.zzzzZ'",
                    "An error occured. Please check the message for further information."));
            }
        }
    }
}
