﻿using System;

namespace ecruise.Database.Models
{
    public partial class CustomerToken
    {
        public uint CustomerTokenId { get; set; }
        public uint CustomerId { get; set; }
        public TokenType Type { get; set; }
        public string Token { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        public virtual Customer Customer { get; set; }
    }

    public enum TokenType
    {
        EmailActivation = 1,
        Login = 2
    }
}
