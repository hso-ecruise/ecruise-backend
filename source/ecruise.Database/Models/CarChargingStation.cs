using System;

namespace ecruise.Database.Models
{
    public partial class CarChargingStation
    {
        public uint CarChargingStationId { get; set; }
        public uint CarId { get; set; }
        public uint ChargingStationId { get; set; }
        public DateTime ChargeStart { get; set; }
        public DateTime? ChargeEnd { get; set; }

        public virtual Car Car { get; set; }
        public virtual ChargingStation ChargingStation { get; set; }
    }
}
