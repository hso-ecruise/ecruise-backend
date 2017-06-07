using System;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public class CarChargingStation
    {
        public ulong CarChargingStationId { get; set; }
        public ulong CarId { get; set; }
        public ulong ChargingStationId { get; set; }
        public DateTime ChargeStart { get; set; }
        public DateTime? ChargeEnd { get; set; }

        [JsonIgnore]
        public virtual Car Car { get; set; }

        [JsonIgnore]
        public virtual ChargingStation ChargingStation { get; set; }
    }
}
