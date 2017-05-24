using System;
using System.Collections.Generic;
using System.Globalization;
using ecruise.Models;
using Microsoft.AspNetCore.Mvc;

namespace ecruise.Api.Controllers
{
    public class BookingsController : BaseController
    {
        // GET: /Bookings/5
        [HttpGet("{id}", Name = "GetBooking")]
        public IActionResult Get(uint id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));

            var bookingEntity = Context.Bookings.Find(id);

            if (bookingEntity == null)
                return NotFound(new Error(1, "Booking with requested booking id does not exist.", "An error occured. Please check the message for further information."));

            return Ok(bookingEntity);
        }

        // POST: /Bookings
        [HttpPost(Name = "PostBooking")]
        public IActionResult Post([FromBody]Booking booking)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/Bookings/1",
                    new PostReference(booking.BookingId, "api/Bookings/"));
            else
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
        }

        // GET: /Bookings/by-trip/5
        [HttpGet("by-trip/{tripid}", Name = "GetBookingsByTrip")]
        public IActionResult GetByTripId(uint tripid)
        {
            if (ModelState.IsValid && tripid < 3 && tripid > 0)
            {
                DateTime date1 = new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc);
                DateTime date2 = new DateTime(2017, 5, 10, 13, 37, 0, DateTimeKind.Utc);

                Booking booking = new Booking(1, 1, tripid, 1, 49.488342, 8.466788, date1,
                    date2);

                return Ok(booking);
            }
            else if (ModelState.IsValid && tripid >= 3)
            {
                return NotFound("Booking with requested trip id does not exist.");
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. TripId must be unsigned int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /Bookings/by-customer/5
        [HttpGet("by-customer/{customerid}", Name = "GetBookingsByCustomer")]
        public IActionResult GetByCustomerId(uint customerid)
        {
            if (ModelState.IsValid && customerid < 3 && customerid > 0)
            {
                DateTime date1 = new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc);
                DateTime date2 = new DateTime(2017, 5, 10, 13, 37, 0, DateTimeKind.Utc);

                Booking booking = new Booking(1, customerid, 1, 1, 49.488342, 8.466788, date1,
                    date2);

                Booking booking2 = new Booking(1, customerid, 1, 1, 49.488342, 8.466788, date1,
                    date2);

                List<Booking> list = new List<Booking> { booking, booking2 };

                return Ok(list);
            }
            else if (ModelState.IsValid && customerid >= 3)
            {
                return NotFound("Booking with requested customer id does not exist.");
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. CustomerId must be unsigned int greater than zero",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /Bookings/by-date/<date>
        [HttpGet("by-date/{date}", Name = "GetBookingsByDate")]
        public IActionResult GetByDate(string date)
        {
            // Transform string to date
            DateTime requestedDateTime;
            if (DateTime.TryParseExact(date, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out requestedDateTime))
            {
                DateTime date1 = new DateTime(2017, 5, 8, 13, 37, 0, DateTimeKind.Utc);

                Booking booking1 = new Booking(1, 1, 1, 1, 49.488342, 8.466788, date1,
                    requestedDateTime);
                Booking booking2 = new Booking(2, 1, 1, 1, 49.488342, 8.466788, date1,
                    requestedDateTime);

                List<Booking> list = new List<Booking> { booking1, booking2 };
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
