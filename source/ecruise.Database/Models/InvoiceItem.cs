using System.Collections.Generic;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public partial class InvoiceItem
    {
        public InvoiceItem()
        {
            Booking = new HashSet<Booking>();
            CarMaintenance = new HashSet<CarMaintenance>();
        }

        public ulong InvoiceItemId { get; set; }
        public ulong? InvoiceId { get; set; }
        public string Reason { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }

        [JsonIgnore]
        public virtual Invoice Invoice { set; get; }


        [JsonIgnore]
        public virtual ICollection<CarMaintenance> CarMaintenance { get; set; }

        [JsonIgnore]
        public virtual ICollection<Booking> Booking { get; set; }
    }

    public enum InvoiceItemType
    {
        Debit = 1,
        Credit
    }
}
