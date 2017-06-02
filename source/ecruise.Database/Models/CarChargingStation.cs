using System;

namespace ecruise.Database.Models
{
    public partial class CarChargingStation
    {
        public ulong CarChargingStationId { get; set; }
        public ulong CarId { get; set; }
        public ulong ChargingStationId { get; set; }
        public DateTime ChargeStart { get; set; }
        public DateTime? ChargeEnd { get; set; }

        public virtual Car Car { get; set; }
        public virtual ChargingStation ChargingStation { get; set; }
    }
}
