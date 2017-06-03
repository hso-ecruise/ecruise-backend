using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Booking = ecruise.Models.Booking;

namespace ecruise.Api.Controllers
{
    public class BookingsController : BaseController
    {

        // GET: /Bookings
        [HttpGet(Name = "GetAllBookings")]
        public IActionResult GetAll()
        {
            try
            {
                // Get all bookings from database
                var bookings = Context.Bookings.ToImmutableList();

                if (bookings.Count < 1)
                    // Return that there are no results
                    return NoContent();

                else
                {
                    // Return found bookings
                    return Ok(BookingAssembler.AssembleModelList(bookings));
                }   
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // GET: /Bookings/5
        [HttpGet("{id}", Name = "GetBooking")]
        public IActionResult Get(ulong id)
        {
            // Check for correct value
            if (!ModelState.IsValid)
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));

            // Get booking from database
            var booking = Context.Bookings.Find(id);    // DEBUG Check for return when no booking found

            if (booking != null)
                return Ok(BookingAssembler.AssembleModel(booking));

            else
                return NotFound(new Error(201, "Booking with requested booking id does not exist.", "An error occured. Please check the message for further information."));
        }

        // POST: /Bookings
        [HttpPost(Name = "PostBooking")]
        public IActionResult Post([FromBody]Booking booking)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check booking for logical validity
                    // Check customer
                    if (Context.Customers.Find(booking.CustomerId) == null)
                        return NotFound(new Error(202, "The customer id referenced in the booking does not exist.",
                            "An error occured. Please check the message for further information."));

                    // Check planned date if set
                    if (booking.PlannedDate != null && booking.PlannedDate < DateTime.UtcNow.AddMinutes(-5))
                    {
                        // The booking must be planned for the future (subtracting 5 minutes e.g. if some latency issues occur)
                        return BadRequest(new Error(302, "PlannedDate must be in the future.",
                            "The DateTime wasn't set properly. Please check the message for further information."));
                    }

                    // Force null on items that cant already be set
                    booking.TripId = null;
                    booking.InvoiceItemId = null;

                    // Construct entity from model
                    Database.Models.Booking bookingEntity = BookingAssembler.AssembleEntity(0, booking);

                    // Save to database
                    Context.Bookings.Add(bookingEntity);
                    Context.SaveChanges();

                    // Get the reference to the newly created entity
                    PostReference pr = new PostReference((uint) bookingEntity.BookingId,
                        $"{BasePath}/bookings/{bookingEntity.BookingId}");

                    // Return reference to the new object including the path to it
                    return Created($"{BasePath}/bookings/{bookingEntity.BookingId}", pr);
                }
                else
                    return BadRequest(new Error(301, GetModelStateErrorString(),
                        "The given data could not be converted to a booking. Please check the message for further information."));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // GET: /Bookings/by-trip/5
        [HttpGet("by-trip/{tripId}", Name = "GetBookingsByTrip")]
        public IActionResult GetByTripId(ulong tripId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get all bookings with the given trip id
                    var bookingEntities = Context.Bookings.Where(b => b.TripId.HasValue && b.TripId.Value == tripId).ToImmutableList();

                    if (bookingEntities.Count < 1)
                        return NoContent();

                    // Convert them to models and return OK
                    return Ok(BookingAssembler.AssembleModelList(bookingEntities));
                }
                else
                    return BadRequest(new Error(301, GetModelStateErrorString(),
                        "The given id could not be converted. Please check the message for further information."));
            }
            catch(Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // GET: /Bookings/by-customer/5
        [HttpGet("by-customer/{customerId}", Name = "GetBookingsByCustomer")]
        public IActionResult GetByCustomerId(ulong customerId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get all bookings with the given trip id
                    var bookingEntities = Context.Bookings.Where(b => b.CustomerId == customerId).ToImmutableList();

                    if (bookingEntities.Count < 1)
                        return NoContent();

                    // Convert them to models and return OK
                    return Ok(BookingAssembler.AssembleModelList(bookingEntities));
                }
                else
                    return BadRequest(new Error(301, GetModelStateErrorString(),
                        "The given id could not be converted. Please check the message for further information."));
            }
            catch (Exception e)
            {
                // return Internal Server Error (500)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Error(101, e.Message, "An error occured.Please check the message for further information."));
            }
        }

        // GET: /Bookings/by-booking-date/<date>
        [HttpGet("by-booking-date/{date}", Name = "GetBookingsByBookingDate")]
        public IActionResult GetByBookingDate(string date)
        {
            // Transform string to date
            DateTime requestedDateTime;
            if (DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out requestedDateTime))
            {
                // Get all bookings booked at the specified day
                var matchingbookings = Context.Bookings
                    .Where(b => b.BookingDate.ToUniversalTime() == requestedDateTime.ToUniversalTime())
                    .ToImmutableList();

                // Check if any matches were found
                if (matchingbookings.Count < 1)
                    return NoContent();

                // Return the found matches
                return Ok(BookingAssembler.AssembleModelList(matchingbookings));
            }
            else
            {
                return BadRequest(new Error(301, "The date given was not formatted correctly.",
                    "Date must always be in following format: 'dd-MM-yyyyTHH:mm:ss.zzzZ'"));
            }
        }

        // GET: /Bookings/by-planned-date/<date>
        [HttpGet("by-planned-date/{date}", Name = "GetBookingsByPlannedDate")]
        public IActionResult GetByPlannedDate(string date)
        {
            // Transform string to date
            DateTime requestedDateTime;
            if (DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out requestedDateTime))
            {
                // Get all bookings booked at the specified day
                var matchingbookings = Context.Bookings
                    .Where(b => b.PlannedDate.HasValue && b.PlannedDate.Value.ToUniversalTime() == requestedDateTime.ToUniversalTime())
                    .ToImmutableList();

                // Check if any matches were found
                if (matchingbookings.Count < 1)
                    return NoContent();

                // Return the found matches
                return Ok(BookingAssembler.AssembleModelList(matchingbookings));
            }
            else
            {
                return BadRequest(new Error(301, "The date given was not formatted correctly.",
                    "Date must always be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'"));
            }
        }
    }
}
