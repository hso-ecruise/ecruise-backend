using System;

namespace ecruise.Database.Models
{
    public partial class CarMaintenance
    {
        public uint CarMaintenanceId { get; set; }
        public uint CarId { get; set; }
        public uint MaintenanceId { get; set; }
        public uint? InvoiceItemId { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public virtual Car Car { get; set; }
        public virtual Maintenance Maintenance { get; set; }
        public virtual InvoiceItem InvoiceItem { get; set; }
    }
}
