using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Statistic
    {
        public DateTime Date { get; set; }
        public double AverageChargeLevel { get; set; }
        public int Bookings { get; set; }
    }
}
