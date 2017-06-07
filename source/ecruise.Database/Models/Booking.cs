using System;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public class Booking
    {
        public ulong BookingId { get; set; }
        public ulong CustomerId { get; set; }
        public ulong? TripId { get; set; }
        public ulong? InvoiceItemId { get; set; }
        public double BookedPositionLatitude { get; set; }
        public double BookedPositionLongitude { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? PlannedDate { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        [JsonIgnore]
        public virtual Trip Trip { get; set; }

        [JsonIgnore]
        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
