﻿using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Maintenance
    {
        public Maintenance()
        {
            CarMaintenance = new HashSet<CarMaintenance>();
        }

        public uint MaintenanceId { get; set; }
        public bool Spontaneously { get; set; }
        public uint? AtMileage { get; set; }
        public DateTime? AtDate { get; set; }

        public virtual ICollection<CarMaintenance> CarMaintenance { get; set; }
    }
}
