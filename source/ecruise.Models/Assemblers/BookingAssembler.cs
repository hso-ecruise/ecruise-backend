using System.Collections.Generic;
using System.Linq;

using Booking = ecruise.Models.Booking;
using DbBooking = ecruise.Database.Models.Booking;

namespace ecruise.Models.Assemblers
{
    public class BookingAssembler
    {
        public static DbBooking AssembleEntity(Booking bookingModel)
        {
            DbBooking bookingEntity =
                new DbBooking
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

        public static Booking AssembleModel(DbBooking bookingEntity)
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

        public static List<Booking> AssembleModelList(IList<DbBooking> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbBooking> AssembleEntityList(IList<Booking> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}
