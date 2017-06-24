using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorLight;
using Booking = ecruise.Models.Booking;

namespace ecruise.Api.Controllers
{
    public class BookingsController : BaseController
    {
        private readonly IRazorLightEngine _razorEngine;

        public BookingsController(EcruiseContext context, IRazorLightEngine razorEngine) : base(context)
        {
            _razorEngine = razorEngine;
        }

        // GET: /Bookings
        [HttpGet(Name = "GetAllBookings")]
        public async Task<IActionResult> GetAllAsync()
        {
            // Get all bookings from database
            var bookings = await Context.Bookings
                // query only bookings the current customer has access to
                .Where(b => HasAccess(b.CustomerId))
                .ToListAsync();

            if (bookings.Count < 1)
                // Return that there are no results
                return NoContent();

            // Return found bookings
            return Ok(BookingAssembler.AssembleModelList(bookings));
        }

        // GET: /Bookings/5
        [HttpGet("{id}", Name = "GetBooking")]
        public async Task<IActionResult> GetAsync(ulong id)
        {
            // Check for correct value
            if (!ModelState.IsValid)
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));

            // Get booking from database
            var booking = await Context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound(new Error(201, "Booking with requested booking id does not exist.",
                    "An error occured. Please check the message for further information."));

            // forbid if current customer is accessing a different user's booking
            if (!HasAccess(booking.CustomerId))
                return Unauthorized();

            return Ok(BookingAssembler.AssembleModel(booking));
        }

        // POST: /Bookings
        [HttpPost(Name = "PostBooking")]
        public async Task<IActionResult> PostAsync([FromBody] Booking booking)
        {
            // Check if new bookings are allowed
            var config = await Context.Configurations.FindAsync((ulong)1);
            if (!config.AllowNewBookings)
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new Error(501, "Currently are no new bookings allowed",
                        "An error occured.Please check the message for further information."));

            // Check for correct value
            if (!ModelState.IsValid)
                return BadRequest(new Error(301, GetModelStateErrorString(),
                    "The given data could not be converted to a booking. Please check the message for further information."));

            // Check booking for logical validity
            // Check customer
            var dbCustomer = await Context.Customers.FindAsync((ulong)booking.CustomerId);
            if (dbCustomer == null)
                return NotFound(new Error(202, "The customer id referenced in the booking does not exist.",
                    "An error occured. Please check the message for further information."));

            if (dbCustomer.Activated == false || dbCustomer.Verified == false)
                return Unauthorized();

            // forbid if current customer is accessing a different user's booking
            if (!HasAccess(booking.CustomerId))
                return Unauthorized();

            // Check planned date if set
            if (booking.PlannedDate != null && booking.PlannedDate < DateTime.UtcNow.AddMinutes(-5))
                // The booking must be planned for the future (subtracting 5 minutes e.g. if some latency issues occur)
                return BadRequest(new Error(302, "PlannedDate must be in the future.",
                    "The DateTime wasn't set properly. Please check the message for further information."));

            // Check if the trip already exists
            if (booking.TripId != null && booking.TripId != 0)
            {
                // Get the trip
                var trip = await Context.Trips.FindAsync((ulong)booking.TripId);

                if (trip == null)
                    return NotFound(new Error(202, "The trip id referenced in the booking does not exist.",
                        "An error occured. Please check the message for further information."));
            }
            else if (booking.TripId == 0)
            {
                booking.TripId = null;
            }

            booking.InvoiceItemId = null;

            // Construct entity from model
            Database.Models.Booking bookingEntity = BookingAssembler.AssembleEntity(0, booking);

            // Set booking date to current time
            bookingEntity.BookingDate = DateTime.UtcNow;

            // Save to database
            await Context.Bookings.AddAsync(bookingEntity);
            await Context.SaveChangesAsync();

            var customer = CustomerAssembler.AssembleModel(dbCustomer);

            if (booking.PlannedDate.HasValue)
            {
                try
                {
                    await customer.SendMail("Deine Registrierung bei eCruise",
                        _razorEngine.Parse("BookingConfirmed.cshtml", new
                        {
                            customer.FirstName,
                            booking.BookingDate,
                            booking.PlannedDate,
                            booking.BookingPositionLatitude,
                            booking.BookingPositionLongitude
                        })
                    );
                }
                catch (Exception e)
                {
                    Debug.WriteLine(
                        $"Booking with id {booking.BookingId} created, but email sending to {customer.FirstName} {customer.LastName} with mail address {customer.Email} failed.\nComplete exception message: {e.Message}",
                        "WARNING");
                }
            }

            // Get the reference to the newly created entity
            PostReference pr = new PostReference((uint)bookingEntity.BookingId,
                $"{BasePath}/bookings/{bookingEntity.BookingId}");

            // Return reference to the new object including the path to it
            return Created($"{BasePath}/bookings/{bookingEntity.BookingId}", pr);
        }

        // GET: /Bookings/by-trip/5
        [HttpGet("by-trip/{tripId}", Name = "GetBookingsByTrip")]
        public async Task<IActionResult> GetByTripIdAsync(ulong tripId)
        {
            // Check for correct value
            if (!ModelState.IsValid)
                return BadRequest(new Error(301, GetModelStateErrorString(),
                    "The given id could not be converted. Please check the message for further information."));

            // Get all bookings with the given trip id
            var bookingEntities = await Context.Bookings
                .Where(b => b.TripId.HasValue && b.TripId.Value == tripId)
                // query only bookings the current customer has access to
                .Where(b => HasAccess(b.CustomerId))
                .ToListAsync();

            if (bookingEntities.Count < 1)
                return NoContent();

            // Convert them to models and return OK
            return Ok(BookingAssembler.AssembleModelList(bookingEntities));
        }

        // GET: /Bookings/by-customer/5
        [HttpGet("by-customer/{customerId}", Name = "GetBookingsByCustomer")]
        public IActionResult GetByCustomerIdAsync(ulong customerId)
        {
            // Check for correct value
            if (!ModelState.IsValid)
                return BadRequest(new Error(301, GetModelStateErrorString(),
                    "The given id could not be converted. Please check the message for further information."));

            // forbid if current customer is accessing a different user's booking
            if (!HasAccess(customerId))
                return Unauthorized();

            // Get all bookings with the given trip id
            var bookingEntities = Context.Bookings.Where(b => b.CustomerId == customerId).ToImmutableList();

            if (bookingEntities.Count < 1)
                return NoContent();

            // Convert them to models and return OK
            return Ok(BookingAssembler.AssembleModelList(bookingEntities));
        }

        // GET: /Bookings/by-booking-date/<date>
        [HttpGet("by-booking-date/{date}", Name = "GetBookingsByBookingDate")]
        public async Task<IActionResult> GetByBookingDateAsync(string date)
        {
            // Transform string to date
            DateTime requestedDateTime;
            if (!DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out requestedDateTime))
                return BadRequest(new Error(301, "The date given was not formatted correctly.",
                    "Date must always be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'"));

            // Set under and upper limit for the day
            DateTime startDate = requestedDateTime.Date;
            DateTime endDate = startDate.AddDays(1).AddMilliseconds(-1);

            // Get all bookings booked at the specified day
            var matchingbookings = await Context.Bookings
                // query only bookings the current customer has access to
                .Where(b => HasAccess(b.CustomerId))
                // filter by date
                .Where(b => b.BookingDate.ToUniversalTime() >= startDate.ToUniversalTime() &&
                            b.BookingDate.ToUniversalTime() <= endDate.ToUniversalTime())
                .ToListAsync();

            // Check if any matches were found
            if (matchingbookings.Count < 1)
                return NoContent();

            // Return the found matches
            return Ok(BookingAssembler.AssembleModelList(matchingbookings));
        }

        // GET: /Bookings/by-planned-date/<date>
        [HttpGet("by-planned-date/{date}", Name = "GetBookingsByPlannedDate")]
        public async Task<IActionResult> GetByPlannedDateAsync(string date)
        {
            // Transform string to date
            DateTime requestedDateTime;
            if (!DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out requestedDateTime))
                return BadRequest(new Error(301, "The date given was not formatted correctly.",
                    "Date must always be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'"));

            // Set under and upper limit for the day
            DateTime startDate = requestedDateTime.Date;
            DateTime endDate = startDate.AddDays(1).AddMilliseconds(-1);

            // Get all bookings booked at the specified day
            var matchingbookings = await Context.Bookings
                // query only bookings the current customer has access to
                .Where(b => HasAccess(b.CustomerId))
                // filter by planned date
                .Where(b => b.BookingDate.ToUniversalTime() >= startDate.ToUniversalTime() &&
                            b.BookingDate.ToUniversalTime() <= endDate.ToUniversalTime())
                .ToListAsync();

            // Check if any matches were found
            if (matchingbookings.Count < 1)
                return NoContent();

            // Return the found matches
            return Ok(BookingAssembler.AssembleModelList(matchingbookings));
        }
    }
}
