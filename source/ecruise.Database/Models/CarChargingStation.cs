using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class CarChargingStation
    {
        public int CarChargingStationId { get; set; }
        public int CarId { get; set; }
        public DateTime? ChargeEnd { get; set; }
        public DateTime ChargeStart { get; set; }
        public int ChargingStationId { get; set; }

        public virtual Car Car { get; set; }
        public virtual ChargingStation ChargingStation { get; set; }
    }
}
