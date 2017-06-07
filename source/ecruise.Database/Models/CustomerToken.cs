using System;
using Newtonsoft.Json;

namespace ecruise.Database.Models
{
    public class CustomerToken
    {
        public ulong CustomerTokenId { get; set; }
        public ulong CustomerId { get; set; }
        public string Type { get; set; }
        public string Token { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }
    }

    public enum TokenType
    {
        EmailActivation = 1,
        Login = 2,
        EmailChangePhase1 = 3,
        EmailChangePhase2 = 4
    }
}
