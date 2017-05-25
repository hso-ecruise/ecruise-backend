using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceItem = new HashSet<InvoiceItem>();
        }

        public uint InvoiceId { get; set; }
        public bool Payed { get; set; }
        public double TotalAmount { get; set; }
        
        public virtual ICollection<InvoiceItem> InvoiceItem { get; set; }
    }
}
