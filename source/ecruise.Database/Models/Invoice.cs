using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            InvoiceItem = new HashSet<InvoiceItem>();
        }

        public ulong InvoiceId { get; set; }
        public ulong CustomerId { get; set; }
        public double TotalAmount { get; set; }
        public bool Paid { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<InvoiceItem> InvoiceItem { get; set; }
    }
}
