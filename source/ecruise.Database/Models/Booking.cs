using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public partial class Booking
    {
        public ulong BookingId { get; set; }
        public ulong CustomerId { get; set; }
        public ulong? TripId { get; set; }
        public ulong? InvoiceItemId { get; set; }
        public double BookedPositionLatitude { get; set; }
        public double BookedPositionLongitude { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? PlannedDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
