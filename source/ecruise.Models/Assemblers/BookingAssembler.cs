using System.Collections.Generic;
using System.Linq;
using DbBooking = ecruise.Database.Models.Booking;

namespace ecruise.Models.Assemblers
{
    public static class BookingAssembler
    {
        public static DbBooking AssembleEntity(ulong id, Booking bookingModel)
        {
            DbBooking bookingEntity =
                new DbBooking
                {
                    BookingId = id != 0 ? id : bookingModel.BookingId,
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
                (uint)bookingEntity.BookingId,
                (uint)bookingEntity.CustomerId,
                (uint?)bookingEntity.TripId,
                (uint?)bookingEntity.InvoiceItemId,
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

        public static List<DbBooking> AssembleEntityList(bool setIdsNull, IList<Booking> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            return models.Select(e => AssembleEntity(e.BookingId, e)).ToList();
        }
    }
}
