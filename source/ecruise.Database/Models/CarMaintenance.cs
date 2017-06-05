using System;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public partial class CarMaintenance
    {
        public ulong CarMaintenanceId { get; set; }
        public ulong CarId { get; set; }
        public ulong MaintenanceId { get; set; }
        public ulong? InvoiceItemId { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        [JsonIgnore]
        public virtual Car Car { get; set; }

        [JsonIgnore]
        public virtual Maintenance Maintenance { get; set; }

        [JsonIgnore]
        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
