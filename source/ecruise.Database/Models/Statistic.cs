using System;

namespace ecruise.Database.Models
{
    public partial class Statistic
    {
        public DateTime Date { get; set; }
        public uint Bookings { get; set; }
        public double AverageChargeLevel { get; set; }
    }
}
