using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceItem = new HashSet<InvoiceItem>();
            Customer = new HashSet<Customer>();
        }

        public uint InvoiceId { get; set; }
        public uint CustomerId { get; set; }
        public bool Payed { get; set; }
        public double TotalAmount { get; set; }
        
        public virtual ICollection<InvoiceItem> InvoiceItem { get; set; }
        public virtual ICollection<Customer> Customer { get; set; }
    }
}
