using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ecruise.Database.Models
{
    public partial class CustomerToken
    {
        public int CustomerTokenId { get; set; }
        public int CustomerId { get; set; }
        public TokenType Type { get; set; }
        public string Token { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        public virtual Customer Customer { get; set; }
    }

    public enum TokenType
    {
        EmailActivation = 0,
        Login = 1
    }
}
