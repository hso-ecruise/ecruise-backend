using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Car
    {
        public Car()
        {
            CarChargingStation = new HashSet<CarChargingStation>();
            CarMaintenance = new HashSet<CarMaintenance>();
            Trip = new HashSet<Trip>();
        }

        public int CarId { get; set; }
        public double ChargeLevel { get; set; }
        public int Kilowatts { get; set; }
        public DateTime? LastKnownPositionDate { get; set; }
        public double? LastKnownPositionLatitude { get; set; }
        public double? LastKnownPositionLongitude { get; set; }
        public string LicensePlate { get; set; }
        public string Manufacturer { get; set; }
        public int Milage { get; set; }
        public string Model { get; set; }

        public virtual ICollection<CarChargingStation> CarChargingStation { get; set; }
        public virtual ICollection<CarMaintenance> CarMaintenance { get; set; }
        public virtual ICollection<Trip> Trip { get; set; }
    }
}
