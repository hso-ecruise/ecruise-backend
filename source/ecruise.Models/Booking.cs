using System;
using System.ComponentModel.DataAnnotations;
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
        /// <param name="invoiceItemId">InvoiceItemId (required)</param>
        /// <param name="bookingPositionLatitude">BookingPositionLatitude (required)</param>
        /// <param name="bookingPositionLongitude">BookingPositionLongitude (required)</param>
        /// <param name="bookingDate">BookingDate (required)</param>
        /// <param name="plannedDate">PlannedDate</param>
        public Booking(uint bookingId, uint customerId, uint? tripId, uint invoiceItemId, double bookingPositionLatitude,
            double bookingPositionLongitude, DateTime bookingDate, DateTime? plannedDate)
        {
            BookingId = bookingId;
            CustomerId = customerId;
            TripId = tripId;
            InvoiceItemId = invoiceItemId;
            BookingPositionLatitude = bookingPositionLongitude;
            BookingPositionLongitude = bookingPositionLongitude;
            BookingDate = bookingDate;
            PlannedDate = plannedDate;
        }

        [Range(1, uint.MaxValue)]
        public uint BookingId { get; }

        [Required, Range(1, uint.MaxValue)]
        public uint CustomerId { get; }

        [Range(1, uint.MaxValue)]
        public uint? TripId { get; set; }

        [Required, Range(1, uint.MaxValue)]
        public uint InvoiceItemId { get; }

        [Required]
        public double BookingPositionLatitude { get; }

        [Required]
        public double BookingPositionLongitude { get; }

        [Required, DataType(DataType.DateTime)]
        public DateTime BookingDate { get; }

        [DataType(DataType.DateTime)]
        public DateTime? PlannedDate { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Booking {\n");
            sb.Append("  BookingId: ").Append(BookingId).Append("\n");
            sb.Append("  CustomerId: ").Append(CustomerId).Append("\n");
            sb.Append("  TripId: ").Append(TripId).Append("\n");
            sb.Append("  InvoiceItemId: ").Append(InvoiceItemId).Append("\n");
            sb.Append("  BookingPositionLatutude: ").Append(BookingPositionLatitude).Append("\n");
            sb.Append("  BookingPositionLongitude: ").Append(BookingPositionLongitude).Append("\n");
            sb.Append("  BookingDate: ").Append(BookingDate.ToString("o")).Append("\n");
            sb.Append("  PlannedDate: ").Append(PlannedDate?.ToString("o")).Append("\n");
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
                (InvoiceItemId == other.InvoiceItemId || InvoiceItemId.Equals(other.InvoiceItemId)) &&
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
                hash = hash * 59 + InvoiceItemId.GetHashCode();
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
