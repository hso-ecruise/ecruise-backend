using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class CustomerToken
        : IEquatable<CustomerToken>
    {
        /// <summary>
        /// Defines the CustomerToken Type
        /// </summary>
        public enum TokenTypeEnum
        {
            EmailActivation,
            Login
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerToken" /> class.
        /// </summary>
        /// <param name="customerTokenId">CustomerTokenId (required)</param>
        /// <param name="customerId">CustomerId (required)</param>
        /// <param name="type">Type (required)</param>
        /// <param name="token">Token (required)</param>
        /// <param name="creationDate">CreationDate (required)</param>
        /// <param name="expireDate">ExpireDate</param>
        public CustomerToken(uint customerTokenId, uint customerId, TokenTypeEnum type, string token,
            DateTime creationDate, DateTime? expireDate)
        {
            CustomerTokenId = customerTokenId;
            CustomerId = customerId;
            Type = type;
            Token = token;
            CreationDate = creationDate;
            ExpireDate = expireDate;
        }

        /// <summary>
        /// Gets CustomerTokenId
        /// </summary>
        [Required, Range(0, uint.MaxValue)]
        public uint CustomerTokenId { get; }

        /// <summary>
        /// Gets CustomerId
        /// </summary>
        [Required, Range(1, uint.MaxValue)]
        public uint CustomerId { get; }

        /// <summary>
        /// Gets Type
        /// </summary>
        [Required]
        public TokenTypeEnum Type { get; }

        /// <summary>
        /// Gets Token
        /// </summary>
        [Required, StringLength(128)]
        public string Token { get; }

        /// <summary>
        /// Gets CreationDate
        /// </summary>
        [Required, DataType(DataType.DateTime)]
        public DateTime CreationDate { get; }

        /// <summary>
        /// Gets ExpireDate
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? ExpireDate { get; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CustomerToken {\n");
            sb.Append("  CustomerTokenId: ").Append(CustomerTokenId).Append("\n");
            sb.Append("  CustomerId: ").Append(CustomerId).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  Token: ").Append(Token).Append("\n");
            sb.Append("  CreationDate: ").Append(CreationDate).Append("\n");
            sb.Append("  ExpireDate: ").Append(ExpireDate).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CustomerToken)obj);
        }

        /// <summary>
        /// Returns true if CustomerToken instances are equal
        /// </summary>
        /// <param name="other">Instance of CustomerToken to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CustomerToken other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (CustomerTokenId == other.CustomerTokenId || CustomerTokenId.Equals(other.CustomerTokenId)) &&
                (CustomerId == other.CustomerId || CustomerId.Equals(other.CustomerId)) &&
                (Type == other.Type || Type.Equals(other.Type)) &&
                (Token == other.Token || Token.Equals(other.Token)) &&
                (CreationDate == other.CreationDate || CreationDate.Equals(other.CreationDate)) &&
                (ExpireDate == other.ExpireDate || (ExpireDate.HasValue && ExpireDate.Equals(other.ExpireDate)));
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 41;

                hash = hash * 59 + CustomerTokenId.GetHashCode();
                hash = hash * 59 + CustomerId.GetHashCode();
                hash = hash * 59 + Type.GetHashCode();
                hash = hash * 59 + Token.GetHashCode();
                hash = hash * 59 + CreationDate.GetHashCode();
                if (ExpireDate.HasValue)
                    hash = hash * 59 + ExpireDate.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(CustomerToken left, CustomerToken right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CustomerToken left, CustomerToken right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
