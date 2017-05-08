using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            Booking = new HashSet<Booking>();
            InvoiceItem = new HashSet<InvoiceItem>();
        }

        public int InvoiceId { get; set; }
        public bool Payed { get; set; }
        public double TotalAmount { get; set; }

        public virtual ICollection<Booking> Booking { get; set; }
        public virtual ICollection<InvoiceItem> InvoiceItem { get; set; }
    }
}
