using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class InvoiceItem
        : IEquatable<InvoiceItem>
    {
        /// <summary>
        /// Defines Type Enums
        /// </summary>
        public enum TypeEnum
        {
            Debit,
            Credit
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceItem" /> class.
        /// </summary>
        /// <param name="invoiceItemId">InvoiceItemId (required)</param>
        /// <param name="invoiceId">See #/definitions/Invoice (required)</param>
        /// <param name="reason">Text which will appear on the invoice. Can contain the name of the service or some other reason. (required)</param>
        /// <param name="type">Type (required)</param>
        /// <param name="amount">Amount (required)</param>
        public InvoiceItem(uint invoiceItemId, uint invoiceId, string reason, TypeEnum type, double amount)
        {
            if (invoiceItemId == 0)
                throw new ArgumentNullException(nameof(invoiceItemId) +
                                                " is a required property for InvoiceItem and cannot be zero");
            if (invoiceId == 0)
                throw new ArgumentNullException(nameof(invoiceId) +
                                                " is a required property for InvoiceItem and cannot be zero");
            if (Math.Abs(amount) < 0.0001)
                throw new ArgumentNullException(nameof(amount) +
                                                " is a required property for InvoiceItem and cannot be zero");

            InvoiceItemId = invoiceItemId;
            InvoiceId = invoiceId;
            Reason = reason ??
                     throw new ArgumentNullException(nameof(reason) +
                                                     " is a required property for InvoiceItem and cannot be null");
            Type = type;
            Amount = amount;
        }

        /// <summary>
        /// Gets or Sets InvoiceItemId
        /// </summary>
        public uint InvoiceItemId { get; }

        /// <summary>
        /// See #/definitions/Invoice
        /// </summary>
        /// <value>See #/definitions/Invoice</value>
        public uint InvoiceId { get; }

        /// <summary>
        /// Text which will appear on the invoice. Can contain the name of the service or some other reason. 
        /// </summary>
        /// <value>Text which will appear on the invoice. Can contain the name of the service or some other reason. </value>
        public string Reason { get; }

        /// <summary>
        /// Gets or Sets Type
        /// </summary>
        public TypeEnum Type { get; }

        /// <summary>
        /// Gets or Sets Amount
        /// </summary>
        public double Amount { get; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InvoiceItem {\n");
            sb.Append("  InvoiceItemId: ").Append(InvoiceItemId).Append("\n");
            sb.Append("  InvoiceId: ").Append(InvoiceId).Append("\n");
            sb.Append("  Reason: ").Append(Reason).Append("\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  Amount: ").Append(Amount).Append("\n");
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
            return Equals((InvoiceItem)obj);
        }

        /// <summary>
        /// Returns true if InvoiceItem instances are equal
        /// </summary>
        /// <param name="other">Instance of InvoiceItem to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(InvoiceItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (InvoiceItemId == other.InvoiceItemId || InvoiceItemId.Equals(other.InvoiceItemId)) &&
                (InvoiceId == other.InvoiceId || InvoiceId.Equals(other.InvoiceId)) &&
                (Reason == other.Reason || Reason.Equals(other.Reason)) &&
                (Type == other.Type || Type.Equals(other.Type)) &&
                (Math.Abs(Amount - other.Amount) < 0.0001 || Amount.Equals(other.Amount));
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

                hash = hash * 59 + InvoiceItemId.GetHashCode();
                hash = hash * 59 + InvoiceId.GetHashCode();
                hash = hash * 59 + Reason.GetHashCode();
                hash = hash * 59 + Type.GetHashCode();
                hash = hash * 59 + Amount.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(InvoiceItem left, InvoiceItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InvoiceItem left, InvoiceItem right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
