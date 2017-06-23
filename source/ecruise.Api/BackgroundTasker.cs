using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ecruise.Database.Models;
using ecruise.Models.Assemblers;
using FluentScheduler;
using GeoCoordinatePortable;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using DbInvoice = ecruise.Database.Models.Invoice;
using DbStatistic = ecruise.Database.Models.Statistic;
using Trip = ecruise.Models.Trip;

namespace ecruise.Api
{
    public class BackgroundTasker
    {
        private readonly DbContextOptions _dbContextOptions;

        public BackgroundTasker()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EcruiseContext>();
            optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            _dbContextOptions = optionsBuilder.Options;
        }

        public Registry ScheduleAllTasks()
        {
            // Create registry
            Registry registry = new Registry();

            // Add car reservation module
            registry.Schedule((Action)CarReservator).ToRunEvery(1).Minutes();

            // Add invoice mailing module
            // Setting every 0 month because it then already starts this month
            registry.Schedule((Action)InvoiceMailer).ToRunEvery(0).Months().OnTheLastDay().At(23, 59);

            // Add statistc creation module
            registry.Schedule((Action)StatisticCreator).ToRunEvery(0).Days().At(7, 0);

            // Add invoice creation module
            registry.Schedule((Action)InvoiceCreator).ToRunEvery(0).Months().On(1).At(0, 0);

            // Add CarCharger module
            registry.Schedule((Action)CarCharger).ToRunEvery(1).Minutes();

            return registry;
        }

        /// <summary>
        ///     Checks for bookings that start in less than 30 minutes, reserves a  car for the booking and creates the trip
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private async void CarReservator()
        {
            using (EcruiseContext context = new EcruiseContext(_dbContextOptions))
            {
                // Get all cars to start in the next 30 minutes and dont have started yet
                var startingBookings = context.Bookings.Where(b => b.TripId == null && b.PlannedDate != null &&
                                                                   b.PlannedDate.Value.ToUniversalTime() < DateTime
                                                                       .UtcNow.AddMinutes(30))
                    .ToList();

                // Check null or empty
                if (startingBookings == null || startingBookings.Count < 1)
                    return;

                // Get all fully loaded and free cars
                var allCars = context.Cars.Where(c => c.BookingState == "AVAILABLE" && c.ChargingState == "FULL")
                    .ToList();

                // Remove entries without coordinates set
                allCars.RemoveAll(c => c.LastKnownPositionLatitude.HasValue == false ||
                                       c.LastKnownPositionLongitude.HasValue == false);

                // Search a matching car for every booking
                foreach (var booking in startingBookings)
                {
                    // Create GeoCoordinate for booking
                    GeoCoordinate bookedPosition =
                        new GeoCoordinate(booking.BookedPositionLatitude, booking.BookedPositionLongitude);

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

                        continue;
                    }

                    // Create trip to link car and booking
                    // Get current chargingstation of car
                    var carChargingStations = context.CarChargingStations.Where(ccs => ccs.CarId == closestCar.CarId)
                        .OrderByDescending(ccs => ccs.ChargeStart)
                        .ToList();
                    var matchingCarChargingStation = carChargingStations.FirstOrDefault();

                    // Check null
                    if (matchingCarChargingStation == null)
                    {
                        Console.Error.WriteLine(
                            $"ERROR: CarReservator has not found the matched car with id {closestCar.CarId} for booking with id {booking.BookingId}." +
                            $"Trying on next iteration.");

                        continue;
                    }

                    // Create trip model
                    Trip trip = new Trip(0, (uint)closestCar.CarId, (uint)booking.CustomerId,
                        booking.PlannedDate ?? booking.BookingDate, null,
                        (uint)matchingCarChargingStation.ChargingStationId, null, null);

                    var newtripEntity = TripAssembler.AssembleEntity(0, trip);

                    // Add trip entity to db
                    context.Trips.Add(newtripEntity);

                    // Set car to be booked
                    closestCar.BookingState = "BOOKED";

                    // Save new trip and car state
                    context.SaveChanges();

                    // Set trip id of booking
                    booking.TripId = newtripEntity.TripId;

                    // Save trip id to booking
                    context.SaveChanges();

                    // Send mail to customer that a car was assigned
                    var customer =
                        CustomerAssembler.AssembleModel(await context.Customers.FindAsync(booking.CustomerId));

                    var mailString = "<!DOCTYPE html>" +
                                     "<html>" +
                                     "<head>" +
                                     "<meta charset = \"utf-8\">" +
                                     "</head>" +
                                     "<body>" +
                                     "<div>" +
                                     "<div style = \"white-space: pre-line;\">" +
                                     $"Hallo {customer.FirstName}!<br/>" +
                                     $"Deiner Fahrt am {booking.PlannedDate.Value:f} wurde soeben ein Auto zugeordnet!<br/>" +
                                     $"Dein Auto hat die Nummer {closestCar.CarId}, das Kennzeichen \"{closestCar.LicensePlate}\" " +
                                     $"und steht an der Ladestation {matchingCarChargingStation.ChargingStationId}.<br/>" +
                                     $"Du findest es hier: <a href=" +
                                     $"\"https://www.google.de/maps/place/{closestCar.LastKnownPositionLatitude.ToString().Replace(",", ".")}" +
                                     $",{closestCar.LastKnownPositionLongitude.ToString().Replace(",", ".")}\">" +
                                     $" {closestCar.LastKnownPositionLatitude.ToString().Replace(",", ".")},{closestCar.LastKnownPositionLongitude.ToString().Replace(",", ".")} </a>.<br/>" +
                                     "Viel Spaß wünscht dir<br/>" +
                                     "Dein eCruise-Team!" +
                                     "</div>" +
                                     "</div>" +
                                     "</body>" +
                                     "</html> ";

                    try
                    {
                        await customer.SendMail("eCruise: Auto zugewiesen", mailString);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Caught exception in CarReservator. The car {closestCar.CarId} was reserved, but the mail could not be sent to {customer.Email}. The full exception code: {e.Message}");
                    }
                }

                Debug.WriteLine($"INFO: Car reservator ausgeführt");
            }
        }

        /// <summary>
        ///     Sends the invoice of the current month to a customer if there are entries
        /// </summary>
        private async void InvoiceMailer()
        {
            using (EcruiseContext context = new EcruiseContext(_dbContextOptions))
            {
                DateTime checkLastMonth = DateTime.UtcNow.AddDays(-1);

                DateTime monthStart = new DateTime(checkLastMonth.Year, checkLastMonth.Month, 1, 0, 0, 0,
                    DateTimeKind.Utc);
                DateTime monthEnd = monthStart.AddMonths(1).AddMilliseconds(-1);

                // Find invoices in the range
                // Get all maintenances of the range
                var carMaintenances = context.CarMaintenances.Where(cm => cm.CompletedDate.HasValue &&
                                                                          cm.CompletedDate.Value.ToUniversalTime() >
                                                                          monthStart &&
                                                                          cm.CompletedDate.Value.ToUniversalTime() <
                                                                          monthEnd)
                    .ToImmutableList();

                var bookings = context.Bookings
                    .Where(b => b.BookingDate.ToUniversalTime() > monthStart.AddMonths(-1) &&
                                b.BookingDate.ToUniversalTime() < monthEnd &&
                                b.Trip.EndDate.HasValue)
                    .ToImmutableList();

                List<ulong> invoiceIds = new List<ulong>();

                // Collect all invoice item ids form the entities
                foreach (var entity in carMaintenances)
                    if (entity.InvoiceItemId.HasValue && !invoiceIds.Contains(entity.InvoiceItemId.Value))
                        invoiceIds.Add(entity.InvoiceItemId.Value);
                foreach (var entity in bookings)
                    if (entity.InvoiceItemId.HasValue && !invoiceIds.Contains(entity.InvoiceItemId.Value))
                        invoiceIds.Add(entity.InvoiceItemId.Value);

                // Get the all invoices for the invoice items
                var invoices = context.Invoices
                    .ToImmutableList()
                    .Where(i => invoiceIds.Contains(i.InvoiceId))
                    .ToImmutableList();

                // Get the matching Customers
                List<Tuple<Invoice, Customer>> invoicePairs = new List<Tuple<Invoice, Customer>>();

                // Put them in a list
                foreach (var invoice in invoices)
                {
                    // Find matching customer
                    var customer = context.Customers.Find(invoice.CustomerId);

                    invoicePairs.Add(new Tuple<Invoice, Customer>(invoice, customer));
                }

                // And send the invoice to each customer
                foreach (var tuple in invoicePairs)
                {
                    var customerModel = CustomerAssembler.AssembleModel(tuple.Item2);

                    try
                    {
                        // Send invoice mail to customer
                        await customerModel.SendMail($"Deine Rechnung für {monthStart:MMMM}",
                            $"Hallo {customerModel.FirstName}!<br/><br/>" +
                            $"Dein Rechnungsbetrag für {monthStart:MMMM} beläuft sich auf: €{tuple.Item1.TotalAmount}" +
                            $"Bitte überwese den Betrag innerhalb von 2 Wochen.<br/>" +
                            $"Freundliche Grüße<br/>" +
                            $"Dein eCruise Team");
                    }
                    catch (SmtpCommandException)
                    {
                        // Catch exception if mail recipient is not reachable/existent
                    }

                    Debug.WriteLine($"INFO: Invoice creator ausgeführt");
                }
            }
        }

        /// <summary>
        ///     Creates the statistic for the current day
        /// </summary>
        private async void StatisticCreator()
        {
            using (EcruiseContext context = new EcruiseContext(_dbContextOptions))
            {
                // Get the current date
                DateTime today = DateTime.UtcNow.Date;

                // Get values for statistic
                // Get number of bookings
                var bookingsPlannedToday = await context.Bookings
                    .Where(b => b.PlannedDate.HasValue && b.PlannedDate.Value > today &&
                                b.PlannedDate.Value < today.AddDays(1).AddMilliseconds(-1))
                    .ToListAsync();

                uint countBookingPlannedForToday = (uint)bookingsPlannedToday.Count;

                // Get all cars
                var allCars = await context.Cars.ToListAsync();

                // Calculate avarage charge level
                double averageChargeLevel = 0.0;

                try
                {
                    averageChargeLevel = allCars
                        .Where(c => c.ChargingState != "DISCHARGING")
                        .Select(c => c.ChargeLevel)
                        .Average();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{DateTime.Now}: {nameof(StatisticCreator)} has caught an exception when calculating the average charge level: {e.Message}");
                }

                // Calculate cars in use
                uint carsInUse = (uint)allCars.Count(c => c.ChargingState == "DISCHARGING");

                // Calculate cars charging
                uint carsCharging = (uint)allCars.Count(c => c.ChargingState == "CHARGING");

                // Create Statistic
                DbStatistic newStatistic = new DbStatistic
                {
                    Date = today,
                    Bookings = countBookingPlannedForToday,
                    AverageChargeLevel = averageChargeLevel,
                    CarsInUse = carsInUse,
                    CarsCharging = carsCharging
                };

                await context.Statistics.AddAsync(newStatistic);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        ///     Creates a new empty invoice for every customer
        /// </summary>
        private async void InvoiceCreator()
        {
            using (EcruiseContext context = new EcruiseContext(_dbContextOptions))
            {
                var allCustomers = await context.Customers.ToListAsync();

                List<DbInvoice> newInvoices = new List<DbInvoice>();

                foreach (var customer in allCustomers)
                    // Create a new invoice for each customer
                    newInvoices.Add(new DbInvoice
                    {
                        CustomerId = customer.CustomerId,
                        Paid = false,
                        TotalAmount = 0.0
                    });

                await context.Invoices.AddRangeAsync(newInvoices);
            }
        }

        /// <summary>
        ///     Charges every car which has chargeState "Charging" until full
        /// </summary>
        private async void CarCharger()
        {
            using (EcruiseContext context = new EcruiseContext(_dbContextOptions))
            {
                // Get all charging cars
                var chargingCars = await context.Cars.Where(c => c.ChargingState == "CHARGING").ToListAsync();

                foreach (var chargingCar in chargingCars)
                {
                    // Add 1 percent charge level
                    chargingCar.ChargeLevel += 1.0;

                    // Check if car fully loaded
                    if (chargingCar.ChargeLevel >= 100.0)
                    {
                        chargingCar.ChargeLevel = 100.0;
                        chargingCar.ChargingState = "FULL";

                        // Get the matching car-charging-station
                        var carChargingStation = await context.CarChargingStations
                            .LastAsync(ccs => ccs.CarId == chargingCar.CarId);

                        // Set charge end date to now
                        carChargingStation.ChargeEnd = DateTime.UtcNow;
                    }
                }

                // Save the changes
                await context.SaveChangesAsync();
            }
        }
    }
}
