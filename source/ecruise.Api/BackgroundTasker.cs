using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ecruise.Database.Models;
using ecruise.Models.Assemblers;
using FluentScheduler;
using GeoCoordinatePortable;
using Customer = ecruise.Database.Models.Customer;
using Invoice = ecruise.Database.Models.Invoice;
using InvoiceItem = ecruise.Database.Models.InvoiceItem;

namespace ecruise.Api
{
    public class BackgroundTasker
    {
        // DbContext
        private readonly EcruiseContext _context;

        public BackgroundTasker(EcruiseContext context)
        {
            _context = context;
        }

        public Registry ScheduleAllTasks()
        {
            // Create registry
            Registry registry = new Registry();

            // Add car reservation module
            registry.Schedule((Action)CarReservator).ToRunEvery(1).Minutes();

            // Add invoice mailing module
            // Setting every 0 month because it then already starts this month
            registry.Schedule((Action)InvoiceCreator).ToRunEvery(0).Months().OnTheLastDay().At(23,59);

            return registry;
        }

        /// <summary>
        /// Checks for bookings that start in less than 30 minutes, reserves a  car for the booking and creates the trip
        /// </summary>
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
                {
                    Console.Error.WriteLine(
                        $"WARNING: CarReservator has not found a matching car for booking with id {booking.BookingId}." +
                        $"Trying on next iteration.");

                    return;
                }

                // Create trip to link car and booking
                // Get current chargingstation of car
                var carChargingStations = _context.CarChargingStations.Where(ccs => ccs.CarId == closestCar.CarId).OrderByDescending(ccs => ccs.ChargeStart).ToList();
                var matchingCarChargingStation = carChargingStations.FirstOrDefault();

                // Check null
                if (matchingCarChargingStation == null)
                {
                    Console.Error.WriteLine(
                        $"ERROR: CarReservator has not found the matched car with id {closestCar.CarId} for booking with id {booking.BookingId}." +
                        $"Trying on next iteration.");

                    return;
                }

                // Create trip model
                Models.Trip trip = new Models.Trip(0, (uint)closestCar.CarId, (uint)booking.CustomerId, null, null, null, null, null);

                var newtripEntity = TripAssembler.AssembleEntity(0, trip);

                // Add trip entity to db
                _context.Trips.Add(newtripEntity);

                // Set car to be booked
                closestCar.BookingState = "BOOKED";

                // Save new trip and car state
                _context.SaveChanges();

                // Set trip id of booking
                booking.TripId = newtripEntity.TripId;

                // Save trip id to booking
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Sends the invoice of the current month to a customer if there are entries
        /// </summary>
        private void InvoiceCreator()
        {
            DateTime checkLastMonth = DateTime.UtcNow.AddDays(-1);

            DateTime monthStart = new DateTime(checkLastMonth.Year, checkLastMonth.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime monthEnd = monthStart.AddMonths(1).AddMilliseconds(-1);

            // Find invoices in the range
            // Get all maintenances of the range
            var carMaintenances = _context.CarMaintenances.Where(cm => cm.CompletedDate.HasValue &&
                                                 cm.CompletedDate.Value.ToUniversalTime() > monthStart &&
                                                 cm.CompletedDate.Value.ToUniversalTime() < monthEnd)
                .ToImmutableList();

            var bookings = _context.Bookings.Where(b => b.BookingDate.ToUniversalTime() > monthStart &&
                                         b.BookingDate.ToUniversalTime() < monthEnd)
                .ToImmutableList();

            List<ulong> invoiceIds = new List<ulong>();

            // Collect all invoice item ids form the entities
            foreach (var entity in carMaintenances)
            {
                if (entity.InvoiceItemId.HasValue && invoiceIds.Contains(entity.InvoiceItemId.Value))
                {
                    invoiceIds.Add(entity.InvoiceItemId.Value);
                }
            }
            foreach (var entity in bookings)
            {
                if(entity.InvoiceItemId.HasValue && invoiceIds.Contains(entity.InvoiceItemId.Value))
                {
                    invoiceIds.Add(entity.InvoiceItemId.Value);
                }
            }

            // Get the all invoices for the invoice items
            var invoices = _context.Invoices.Where(i => invoiceIds.Contains(i.InvoiceId)).ToImmutableList();

            // Get the matching Customers
            List<Tuple<Invoice, Customer>> invoicePairs = new List<Tuple<Invoice, Customer>>();

            // Put them in a list
            foreach (var invoice in invoices)
            {
                // Find matching customer
                var customer = _context.Customers.Find(invoice.CustomerId);

                // Check if found
                if (customer == null)
                {
                    Console.Error.WriteLine($"ERROR: The customer with id {invoice.CustomerId} for the invoice with id {invoice.CustomerId} was not found");
                    continue;
                }

                invoicePairs.Add(new Tuple<Invoice, Customer>(invoice, customer));
            }

            // And send the invoice to each customer
            foreach (var tuple in invoicePairs)
            {
                var customerModel = CustomerAssembler.AssembleModel(tuple.Item2);

                List<InvoiceItem> invoiceItemsOfCustomer = new List<InvoiceItem>();

                customerModel.SendMail($"Deine Rechnung für {monthStart:M}", $"Betrag: {tuple.Item1.TotalAmount}");
            }
        }
    }
}
