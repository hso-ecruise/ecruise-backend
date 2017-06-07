using System;

namespace ecruise.Database.Models
{
    public class Statistic
    {
        public DateTime Date { get; set; }
        public uint Bookings { get; set; }
        public double AverageChargeLevel { get; set; }
        public uint CarsInUse { get; set; }
        public uint CarsCharging { get; set; }
    }
}
