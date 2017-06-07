using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public class Maintenance
    {
        public Maintenance()
        {
            CarMaintenance = new HashSet<CarMaintenance>();
        }

        public ulong MaintenanceId { get; set; }
        public bool Spontaneously { get; set; }
        public uint? AtMileage { get; set; }
        public DateTime? AtDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<CarMaintenance> CarMaintenance { get; set; }
    }
}
