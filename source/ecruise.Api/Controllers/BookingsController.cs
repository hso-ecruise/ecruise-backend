using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Booking = ecruise.Models.Booking;

namespace ecruise.Api.Controllers
{
    /// <summary>
    ///     This class handles all requestes related to bookings.
    ///     That means that it handles all web requests to /bookings
    /// </summary>
    public class BookingsController : BaseController
    {
        /// <summary>
        ///     The constructor for the <see cref="BookingsController"/> class.
        /// </summary>
        /// <param name="context">The database context that gets injected by the framework.</param>
        public BookingsController(EcruiseContext context)
            : base(context)
        {
        }

        /// <summary>
        ///     Queries all bookings in the database and returns a http response that indicates if results were found.
        ///     Additionally, if there are any results, it returns a list of <see cref="Booking"/>s.
        /// </summary>
        /// <example>
        ///     GET /bookings
        /// </example>
        /// <returns>
        ///     HTTP 200 Ok: Returns a List of <see cref="Booking"/>s if <see cref="Booking"/>s everything's okay.
        ///     HTTP 204 No Content: There are no <see cref="Booking"/>s.
        /// </returns>
        [HttpGet(Name = "GetAllBookingsAsync")]
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

        /// <summary>
        ///     Queries one <see cref="Booking"/> identified by <paramref name="id"/> and returns it, if existent.
        /// </summary>
        /// <param name="id">The booking id to search for</param>
        /// <example>
        ///     GET /bookings/5
        /// </example>
        /// <returns>
        ///     HTTP 200 Ok: Returns the <see cref="Booking"/> if everything's okay.
        ///     HTTP 400 Bad Request: The provided parameters are malformed.
        ///     HTTP 401 Unauthorized: The authenticated customer has no access to the requested <see cref="Booking"/>.
        ///     HTTP 404 Not Found: There is no booking with such an <paramref name="id"/>.
        /// </returns>
        [HttpGet("{id}", Name = "GetBookingAsync")]
        public async Task<IActionResult> GetAsync(ulong id)
        {
            // Check for correct value
            if (!ModelState.IsValid)
                return BadRequest(new Error(1,
                    "The id given was not formatted correctly. Id must be unsigned int",
                    "An error occured. Please check the message for further information."));

            // Get booking from database
            var booking = await Context.Bookings.FindAsync(id);

            // return error if booking was not found
            if (booking == null)
                return NotFound(new Error(201, "Booking with requested booking id does not exist.",
                    "An error occured. Please check the message for further information."));

            // forbid if current customer is accessing a different user's booking
            if (!HasAccess(booking.CustomerId))
                return Unauthorized();

            return Ok(BookingAssembler.AssembleModel(booking));
        }

        /// <summary>
        ///     Try to create a new <see cref="Booking"/> with the data provided by <paramref name="booking"/>.
        /// </summary>
        /// <example>
        ///     POST /bookings
        /// </example>
        /// <returns>
        ///     HTTP 201 Created: Returns a <see cref="PostReference"/> to the created object on success.
        ///     HTTP 400 Bad Request: The provided parameters are malformed.
        ///     HTTP 401 Unauthorized: The authenticated customer has no access to create the provided <see cref="Booking"/>.
        ///     HTTP 404 Not Found: There is no customer with CustomerId provided in <paramref name="booking"/>.
        /// </returns>
        [HttpPost(Name = "PostBookingAsync")]
        public async Task<IActionResult> PostAsync([FromBody] Booking booking)
        {
            // Check if new bookings are allowed
            var config = Context.Configurations.Find((ulong)1);
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
            if (Context.Customers.Find((ulong)booking.CustomerId) == null)
                return NotFound(new Error(202, "The customer id referenced in the booking does not exist.",
                    "An error occured. Please check the message for further information."));

            // forbid if current customer is accessing a different user's booking
            if (!HasAccess(booking.CustomerId))
                return Unauthorized();

            // Check planned date if set
            if (booking.PlannedDate != null && booking.PlannedDate < DateTime.UtcNow.AddMinutes(-5))
                // The booking must be planned for the future (subtracting 5 minutes e.g. if some latency issues occur)
                return BadRequest(new Error(302, "PlannedDate must be in the future.",
                    "The DateTime wasn't set properly. Please check the message for further information."));

            // Force null on items that cant already be set
            booking.TripId = null;
            booking.InvoiceItemId = null;

            // Construct entity from model
            Database.Models.Booking bookingEntity = BookingAssembler.AssembleEntity(0, booking);

            // Set booking date to current time
            bookingEntity.BookingDate = DateTime.UtcNow;

            // Save to database
            await Context.Bookings.AddAsync(bookingEntity);
            await Context.SaveChangesAsync();

            // Get the reference to the newly created entity
            PostReference pr = new PostReference((uint)bookingEntity.BookingId,
                $"{BasePath}/bookings/{bookingEntity.BookingId}");

            // Return reference to the new object including the path to it
            return Created($"{BasePath}/bookings/{bookingEntity.BookingId}", pr);
        }

        /// <summary>
        ///     Find all <see cref="Booking"/> that belongs to a associated <see cref="Models.Trip"/> 
        ///     identified by it's <paramref name="tripId"/>.
        ///     An error response is set if there's no such related <see cref="Models.Booking"/>.
        /// </summary>
        /// <example>
        ///     GET: /bookings/by-trip/5
        /// </example>
        /// <returns>
        ///     HTTP 200 Ok: Returns the <see cref="Models.Booking"/> associated with the requested <param name="tripId"></param>.
        ///     HTTP 203 No Content: There is no <see cref="Models.Booking"/> that the current customer can access
        ///                          associated with the requested <paramref name="tripId"/>.
        ///     HTTP 400 Bad Request: The provided parameters are malformed.
        /// </returns>
        [HttpGet("by-trip/{tripId}", Name = "GetBookingsByTripAsync")]
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

        /// <summary>
        ///     Find all <see cref="Booking"/> that belongs to a associated <see cref="Models.Customer"/> 
        ///     identified by it's <paramref name="customerId"/>.
        ///     An error response is set if there's no such related <see cref="Models.Booking"/>.
        /// </summary>
        /// <example>
        ///     GET: /bookings/by-customer/5
        /// </example>
        /// <returns>
        ///     HTTP 200 Ok: Returns the <see cref="Models.Booking"/> associated with the requested <param name="customerId"></param>.
        ///     HTTP 203 No Content: There is no <see cref="Models.Booking"/> that the current customer can access associated with 
        ///                          the requested <paramref name="customerId"/>.
        ///     HTTP 400 Bad Request: The provided parameters are malformed.
        /// </returns>
        [HttpGet("by-customer/{customerId}", Name = "GetBookingsByCustomerAsync")]
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

        /// <summary>
        ///     Find all <see cref="Booking"/> that were booked at a certain <paramref name="date"/>.
        ///     An error response is set if there's was <see cref="Models.Booking"/> on that date.
        /// </summary>
        /// <example>
        ///     GET: /Bookings/by-booking-date/{date}
        /// </example>
        /// <returns>
        ///     HTTP 200 Ok: Returns the <see cref="Models.Booking"/> booked at the requested <paramref name="date"/>.
        ///     HTTP 203 No Content: There is no <see cref="Models.Booking"/> that was booked on that date.
        ///     HTTP 400 Bad Request: The provided parameters are malformed.
        /// </returns>
        [HttpGet("by-booking-date/{date}", Name = "GetBookingsByBookingDateAsync")]
        public async Task<IActionResult> GetByBookingDateAsync(string date)
        {
            // Transform string to date
            DateTime requestedDateTime;
            if (!DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out requestedDateTime))
                return BadRequest(new Error(301, "The date given was not formatted correctly.",
                    "Date must always be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'"));

            // Get all bookings booked at the specified day
            var matchingbookings = await Context.Bookings
                // query only bookings the current customer has access to
                .Where(b => HasAccess(b.CustomerId))
                // filter by date
                .Where(b => b.BookingDate.ToUniversalTime() == requestedDateTime.ToUniversalTime())
                .ToListAsync();

            // Check if any matches were found
            if (matchingbookings.Count < 1)
                return NoContent();

            // Return the found matches
            return Ok(BookingAssembler.AssembleModelList(matchingbookings));
        }

        /// <summary>
        ///     Find all <see cref="Booking"/> that were/are planned at a certain <paramref name="date"/>.
        ///     An error response is set if there's was <see cref="Models.Booking"/> on that date.
        /// </summary>
        /// <example>
        ///     GET: /Bookings/by-planned-date/{date}
        /// </example>
        /// <returns>
        ///     HTTP 200 Ok: Returns the <see cref="Models.Booking"/> planned at the requested <paramref name="date"/>.
        ///     HTTP 203 No Content: There is no <see cref="Models.Booking"/> that was/is planned on that date.
        ///     HTTP 400 Bad Request: The provided parameters are malformed.
        /// </returns>
        [HttpGet("by-planned-date/{date}", Name = "GetBookingsByPlannedDateAsync")]
        public async Task<IActionResult> GetByPlannedDateAsync(string date)
        {
            // Transform string to date
            DateTime requestedDateTime;
            if (!DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out requestedDateTime))
                return BadRequest(new Error(301, "The date given was not formatted correctly.",
                    "Date must always be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'"));

            // Get all bookings booked at the specified day
            var matchingbookings = await Context.Bookings
                // query only bookings the current customer has access to
                .Where(b => HasAccess(b.CustomerId))
                // filter by planned date
                .Where(b => b.PlannedDate.HasValue && b.PlannedDate.Value.ToUniversalTime() ==
                            requestedDateTime.ToUniversalTime())
                .ToListAsync();

            // Check if any matches were found
            if (matchingbookings.Count < 1)
                return NoContent();

            // Return the found matches
            return Ok(BookingAssembler.AssembleModelList(matchingbookings));
        }
    }
}
