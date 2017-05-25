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
        public uint Slots { get; set; }
        public uint SlotsOccupied { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public virtual ICollection<CarChargingStation> CarChargingStation { get; set; }
        public virtual ICollection<Trip> TripEndChargingStation { get; set; }
        public virtual ICollection<Trip> TripStartChargingStation { get; set; }
    }
}
