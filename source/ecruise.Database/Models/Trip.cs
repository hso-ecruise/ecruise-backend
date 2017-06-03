using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Trip
    {
        public Trip()
        {
            Booking = new HashSet<Booking>();
        }

        public ulong TripId { get; set; }
        public ulong? CarId { get; set; }
        public ulong CustomerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ulong? StartChargingStationId { get; set; }
        public ulong? EndChargingStationId { get; set; }
        public double? DistanceTravelled { get; set; }

        public virtual ICollection<Booking> Booking { get; set; }
        public virtual Car Car { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ChargingStation EndChargingStation { get; set; }
        public virtual ChargingStation StartChargingStation { get; set; }
    }
}
