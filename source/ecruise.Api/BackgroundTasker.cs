using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using FluentScheduler;
using GeoCoordinatePortable;

namespace ecruise.Api
{
    public class BackgroundTasker
    {
        // DbContext
        private EcruiseContext _context;

        public BackgroundTasker(EcruiseContext context)
        {
            _context = context;
        }

        public void RunAllTasks()
        {
            // Create registry
            Registry registry = new Registry();

            
        }

        private void CarReservator()
        {
            // Get all cars to start in the next 30 minutes and dont have started yet
            var startingBookings = _context.Bookings.Where(b => b.TripId == 0 && b.PlannedDate.HasValue &&
                                         b.PlannedDate.Value.ToUniversalTime() < DateTime.UtcNow.AddMinutes(30)).ToList();

            // Check null or empty
            if (startingBookings == null || startingBookings.Count < 1)
                return;

            // Get all fully loaded and free cars
            var allCars = _context.Cars.Where(c => c.BookingState == "AVAILABLE" && c.ChargingState == "FULL").ToList();

            // Remove entries without coordinates set
            allCars.RemoveAll(c => c.LastKnownPositionLatitude.HasValue == false ||
                                   c.LastKnownPositionLongitude.HasValue == false);

            // Search a matching car for every booking
            foreach (var booking in startingBookings)
            {
                // Create GeoCoordinate for booking
                GeoCoordinate bookedPosition = new GeoCoordinate(booking.BookedPositionLatitude, booking.BookedPositionLongitude);

                // Order cars by distance to the booked position
                allCars = allCars
                    // ReSharper disable once PossibleInvalidOperationException
                    .OrderBy(c => bookedPosition.GetDistanceTo(new GeoCoordinate(c.LastKnownPositionLatitude.Value,
                        // ReSharper disable once PossibleInvalidOperationException
                        c.LastKnownPositionLongitude.Value)))
                    .ToList();

                var closestCar = allCars.FirstOrDefault();

                if (closestCar == null)
                    Console.Error.WriteLine($"WARNING: CarReservator has not found a matching car for booking with id {booking.BookingId}." +
                                            $"Trying on next iteration.");
            }
        }

    }
}
