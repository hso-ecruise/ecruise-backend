using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class CustomerToken
    {
        public int CustomerTokenId { get; set; }
        public DateTime CreationDate { get; set; }
        public int CustomerId { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Token { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
