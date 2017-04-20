using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public double BookedPositionLatitude { get; set; }
        public double BookedPositionLongitude { get; set; }
        public DateTime BookingDate { get; set; }
        public int CustomerId { get; set; }
        public int InvoiceId { get; set; }
        public DateTime? PlannedDate { get; set; }
        public int? TripId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Trip Trip { get; set; }
    }
}
