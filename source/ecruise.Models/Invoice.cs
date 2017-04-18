using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Invoice :  IEquatable<Invoice>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Invoice" /> class.
        /// </summary>
        /// <param name="invoiceId">InvoiceId.</param>
        /// <param name="totalAmount">TotalAmount.</param>
        /// <param name="paid">Paid.</param>
        public Invoice(int? invoiceId, double? totalAmount, bool? paid)
        {
            InvoiceId = invoiceId;
            TotalAmount = totalAmount;
            Paid = paid;
        }

        /// <summary>
        /// Gets or Sets InvoiceId
        /// </summary>
        public int? InvoiceId { get; set; }
        /// <summary>
        /// Gets or Sets TotalAmount
        /// </summary>
        public double? TotalAmount { get; set; }
        /// <summary>
        /// Gets or Sets Paid
        /// </summary>
        public bool? Paid { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Invoice {\n");
            sb.Append("  InvoiceId: ").Append(InvoiceId).Append("\n");
            sb.Append("  TotalAmount: ").Append(TotalAmount).Append("\n");
            sb.Append("  Paid: ").Append(Paid).Append("\n");
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
            return Equals((Invoice)obj);
        }

        /// <summary>
        /// Returns true if Invoice instances are equal
        /// </summary>
        /// <param name="other">Instance of Invoice to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Invoice other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    InvoiceId == other.InvoiceId ||
                    InvoiceId != null &&
                    InvoiceId.Equals(other.InvoiceId)
                ) && 
                (
                    TotalAmount == other.TotalAmount ||
                    TotalAmount != null &&
                    TotalAmount.Equals(other.TotalAmount)
                ) && 
                (
                    Paid == other.Paid ||
                    Paid != null &&
                    Paid.Equals(other.Paid)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                    if (InvoiceId != null)
                    hash = hash * 59 + InvoiceId.GetHashCode();
                    if (TotalAmount != null)
                    hash = hash * 59 + TotalAmount.GetHashCode();
                    if (Paid != null)
                    hash = hash * 59 + Paid.GetHashCode();
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Invoice left, Invoice right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Invoice left, Invoice right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
