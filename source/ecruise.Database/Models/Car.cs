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

        public uint CarId { get; set; }
        public string LicensePlate { get; set; }
        public ChargingState ChargingState { get; set; }
        public BookingState BookingState { get; set; }
        public uint Milage { get; set; }
        public double ChargeLevel { get; set; }
        public uint Kilowatts { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int YearOfConstruction { get; set; }
        public double? LastKnownPositionLatitude { get; set; }
        public double? LastKnownPositionLongitude { get; set; }
        public DateTime? LastKnownPositionDate { get; set; }

        public virtual ICollection<CarChargingStation> CarChargingStation { get; set; }
        public virtual ICollection<CarMaintenance> CarMaintenance { get; set; }
        public virtual ICollection<Trip> Trip { get; set; }
    }

    public enum ChargingState
    {
        Discharging = 1,
        Charging,
        Full
    }

    public enum BookingState
    {
        Available = 1,
        Booked,
        Blocked
    }
}
