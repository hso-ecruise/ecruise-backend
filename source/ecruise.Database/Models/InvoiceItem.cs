using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class InvoiceItem
    {
        public InvoiceItem()
        {
            Booking = new HashSet<Booking>();
            CarMaintenance = new HashSet<CarMaintenance>();
        }

        public uint InvoiceItemId { get; set; }
        public uint? InvoiceId { get; set; }
        public string Reason { get; set; }
        public InvoiceItemType Type { get; set; }
        public double Amount { get; set; }

        public virtual Invoice Invoice { set; get; }

        public virtual ICollection<CarMaintenance> CarMaintenance { get; set; }
        public virtual ICollection<Booking> Booking { get; set; }
    }

    public enum InvoiceItemType
    {
        Debit = 0,
        Credit = 1
    }
}
