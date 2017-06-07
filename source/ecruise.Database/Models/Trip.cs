using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public class Trip
    {
        public Trip()
        {
            Booking = new HashSet<Booking>();
        }

        public ulong TripId { get; set; }
        public ulong? CarId { get; set; }
        public ulong CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ulong StartChargingStationId { get; set; }
        public ulong? EndChargingStationId { get; set; }
        public double? DistanceTravelled { get; set; }

        [JsonIgnore]
        public virtual ICollection<Booking> Booking { get; set; }

        [JsonIgnore]
        public virtual Car Car { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        [JsonIgnore]
        public virtual ChargingStation EndChargingStation { get; set; }

        [JsonIgnore]
        public virtual ChargingStation StartChargingStation { get; set; }
    }
}
