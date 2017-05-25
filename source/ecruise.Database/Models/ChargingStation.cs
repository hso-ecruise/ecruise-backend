using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class ChargingStation
    {
        public ChargingStation()
        {
            CarChargingStation = new HashSet<CarChargingStation>();
            TripEndChargingStation = new HashSet<Trip>();
            TripStartChargingStation = new HashSet<Trip>();
        }

        public uint ChargingStationId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Slots { get; set; }
        public int SlotsOccupied { get; set; }

        public virtual ICollection<CarChargingStation> CarChargingStation { get; set; }
        public virtual ICollection<Trip> TripEndChargingStation { get; set; }
        public virtual ICollection<Trip> TripStartChargingStation { get; set; }
    }
}
