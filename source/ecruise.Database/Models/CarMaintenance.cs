using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class CarMaintenance
    {
        public int CarMaintenanceId { get; set; }
        public int CarId { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? InvoiceItemId { get; set; }
        public int MaintenanceId { get; set; }
        public DateTime? PlannedDate { get; set; }

        public virtual Car Car { get; set; }
        public virtual InvoiceItem InvoiceItem { get; set; }
        public virtual Maintenance Maintenance { get; set; }
    }
}
