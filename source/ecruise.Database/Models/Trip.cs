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

        public int TripId { get; set; }
        public int? CarId { get; set; }
        public int CustomerId { get; set; }
        public double? DistanceTravelled { get; set; }
        public int? EndChargingStationId { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StartChargingStationId { get; set; }
        public DateTime? StartDate { get; set; }

        public virtual ICollection<Booking> Booking { get; set; }
        public virtual Car Car { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ChargingStation EndChargingStation { get; set; }
        public virtual ChargingStation StartChargingStation { get; set; }
    }
}
