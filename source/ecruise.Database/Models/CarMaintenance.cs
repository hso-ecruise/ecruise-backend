using System;

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

        public virtual Car Car { get; set; }
        public virtual Maintenance Maintenance { get; set; }
        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
