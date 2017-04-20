using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Booking
        : IEquatable<Booking>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Booking" /> class.
        /// </summary>
        /// <param name="bookingId">BookingId (required)</param>
        /// <param name="customerId">CustomerId (required)</param>
        /// <param name="tripId">TripId</param>
        /// <param name="invoiceId">InvoiceId (required)</param>
        /// <param name="bookingPositionLatitude">BookingPositionLatitude (required)</param>
        /// <param name="bookingPositionLongitude">BookingPositionLongitude (required)</param>
        /// <param name="bookingDate">BookingDate (required)</param>
        /// <param name="plannedDate">PlannedDate</param>
        public Booking(uint bookingId, uint customerId, uint? tripId, uint invoiceId, double bookingPositionLatitude,
            double bookingPositionLongitude, DateTime bookingDate, DateTime? plannedDate)
        {
            if (bookingId == 0)
                throw new ArgumentNullException(
                    nameof(bookingId) + " is a required property for Booking and cannot be zero");
            if (customerId == 0)
                throw new ArgumentNullException(
                    nameof(customerId) + " is a required property for Booking and cannot be zero");
            if (invoiceId == 0)
                throw new ArgumentNullException(
                    nameof(invoiceId) + " is a required property for Booking and cannot be zero");
            if (Math.Abs(bookingPositionLatitude) < 0.00001)
                throw new ArgumentNullException(
                    nameof(bookingPositionLatitude) + " is a required property for Booking and cannot be zero");

            BookingId = bookingId;
            CustomerId = customerId;
            TripId = tripId;
            InvoiceId = invoiceId;
            BookingPositionLatitude = bookingPositionLongitude;
            BookingPositionLongitude = bookingPositionLongitude;
            BookingDate = bookingDate;
            PlannedDate = plannedDate;
        }

        public uint BookingId { get; }
        public uint CustomerId { get; }
        public uint? TripId { get; set; }
        public uint InvoiceId { get; }
        public double BookingPositionLatitude { get; }
        public double BookingPositionLongitude { get; }
        public DateTime BookingDate { get; }
        public DateTime? PlannedDate { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Booking {\n");
            sb.Append("  BookingId: ").Append(BookingId).Append("\n");
            sb.Append("  CustomerId: ").Append(CustomerId).Append("\n");
            sb.Append("  TripId: ").Append(TripId).Append("\n");
            sb.Append("  InvoiceId: ").Append(InvoiceId).Append("\n");
            sb.Append("  BookingPositionLatutude: ").Append(BookingPositionLatitude).Append("\n");
            sb.Append("  BookingPositionLongitude: ").Append(BookingPositionLongitude).Append("\n");
            sb.Append("  BookingDate: ").Append(BookingDate).Append("\n");
            sb.Append("  PlannedDate: ").Append(PlannedDate).Append("\n");
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
            return Equals((Booking)obj);
        }

        /// <summary>
        /// Returns true if Booking instances are equal
        /// </summary>
        /// <param name="other">Instance of Booking to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Booking other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (BookingId == other.BookingId || BookingId.Equals(other.BookingId)) &&
                (CustomerId == other.CustomerId || CustomerId.Equals(other.CustomerId)) &&
                (TripId == other.TripId || (TripId.HasValue && TripId.Equals(other.TripId))) &&
                (InvoiceId == other.InvoiceId || InvoiceId.Equals(other.InvoiceId)) &&
                (
                    Math.Abs(BookingPositionLatitude - other.BookingPositionLatitude) < 0.00001 ||
                    BookingPositionLatitude.Equals(other.BookingPositionLatitude)
                ) &&
                (
                    Math.Abs(BookingPositionLongitude - other.BookingPositionLongitude) < 0.00001 ||
                    BookingPositionLongitude.Equals(other.BookingPositionLongitude)
                ) &&
                (BookingDate == other.BookingDate || BookingDate.Equals(other.BookingDate)) &&
                (PlannedDate == other.PlannedDate || (PlannedDate.HasValue && PlannedDate.Equals(other.PlannedDate)));
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

                hash = hash * 59 + BookingId.GetHashCode();
                hash = hash * 59 + CustomerId.GetHashCode();
                hash = hash * 59 + InvoiceId.GetHashCode();
                hash = hash * 59 + BookingPositionLongitude.GetHashCode();
                hash = hash * 59 + BookingPositionLatitude.GetHashCode();
                hash = hash * 59 + BookingDate.GetHashCode();
                if (PlannedDate.HasValue)
                    hash = hash * 59 + PlannedDate.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Booking left, Booking right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Booking left, Booking right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
