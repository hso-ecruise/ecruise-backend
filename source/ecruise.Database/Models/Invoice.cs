using System.Collections.Generic;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public class Invoice
    {
        public Invoice()
        {
            InvoiceItem = new HashSet<InvoiceItem>();
        }

        public ulong InvoiceId { get; set; }
        public ulong CustomerId { get; set; }
        public double TotalAmount { get; set; }
        public bool Paid { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        [JsonIgnore]
        public virtual ICollection<InvoiceItem> InvoiceItem { get; set; }
    }
}
