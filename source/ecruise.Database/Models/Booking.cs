﻿using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public partial class Booking
    {
        public uint BookingId { get; set; }
        public uint CustomerId { get; set; }
        public uint? TripId { get; set; }
        public uint? InvoiceItemId { get; set; }
        public double BookedPositionLatitude { get; set; }
        public double BookedPositionLongitude { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? PlannedDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
