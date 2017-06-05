using System.Collections.Generic;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Booking = new HashSet<Booking>();
            CustomerToken = new HashSet<CustomerToken>();
            Trip = new HashSet<Trip>();
            Invoice = new HashSet<Invoice>();
        }

        public ulong CustomerId { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ChipCardUid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public uint? ZipCode { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string AddressExtraLine { get; set; }
        public bool Activated { get; set; }
        public bool Verified { get; set; }

        [JsonIgnore]
        public virtual ICollection<Booking> Booking { get; set; }

        [JsonIgnore]
        public virtual ICollection<CustomerToken> CustomerToken { get; set; }

        [JsonIgnore]
        public virtual ICollection<Trip> Trip { get; set; }

        [JsonIgnore]
        public virtual ICollection<Invoice> Invoice { get; set; }
    }
}
