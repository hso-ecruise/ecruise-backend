using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    class BookingAssembler
    {
        public static Database.Models.Booking AssembleEntity(Booking bookingModel)
        {
            Database.Models.Booking bookingEntity =
                new Database.Models.Booking
                {
                    BookingId = bookingModel.BookingId,
                    CustomerId = bookingModel.CustomerId,
                    TripId = bookingModel.TripId,
                    InvoiceItemId = bookingModel.InvoiceItemId,
                    BookedPositionLatitude = bookingModel.BookingPositionLatitude,
                    BookedPositionLongitude = bookingModel.BookingPositionLongitude,
                    BookingDate = bookingModel.BookingDate,
                    PlannedDate = bookingModel.PlannedDate
                };

            return bookingEntity;
        }

        public static Booking AssembleModel(Database.Models.Booking bookingEntity)
        {
            return new Booking(
                bookingEntity.BookingId,
                bookingEntity.CustomerId,
                bookingEntity.TripId,
                bookingEntity.InvoiceItemId,
                bookingEntity.BookedPositionLatitude,
                bookingEntity.BookedPositionLongitude,
                bookingEntity.BookingDate,
                bookingEntity.PlannedDate
            );
        }
    }
}
