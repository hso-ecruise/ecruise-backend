using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class InvoiceItem
    {
        public InvoiceItem()
        {
            CarMaintenance = new HashSet<CarMaintenance>();
        }

        public int InvoiceItemId { get; set; }
        public double Amount { get; set; }
        public int? InvoiceId { get; set; }
        public string Reason { get; set; }

        public virtual ICollection<CarMaintenance> CarMaintenance { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}
