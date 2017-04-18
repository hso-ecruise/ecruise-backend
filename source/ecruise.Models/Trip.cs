using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Trip 
        : IEquatable<Trip>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trip" /> class.
        /// </summary>
        /// <param name="tripId">TripId.</param>
        /// <param name="carId">See #/definitions/Car.</param>
        /// <param name="customerId">See #/definitions/Customer.</param>
        /// <param name="startDate">Date and time when the trip started.</param>
        /// <param name="endDate">Date and time when the trip ended.</param>
        /// <param name="startPositionLatitude">StartPositionLatitude.</param>
        /// <param name="startPositionLongitude">StartPositionLongitude.</param>
        /// <param name="endPositionLatitude">EndPositionLatitude.</param>
        /// <param name="endPositionLongitude">EndPositionLongitude.</param>
        public Trip(int tripId, int carId, int customerId, DateTime startDate, DateTime? endDate,
            double startPositionLatitude, double startPositionLongitude, double? endPositionLatitude,
            double? endPositionLongitude)
        {
            TripId = tripId;
            CarId = carId;
            CustomerId = customerId;
            StartDate = startDate;
            EndDate = endDate;
            StartPositionLatitude = startPositionLatitude;
            StartPositionLongitude = startPositionLongitude;
            EndPositionLatitude = endPositionLatitude;
            EndPositionLongitude = endPositionLongitude;
        }

        /// <summary>
        /// Gets or Sets TripId
        /// </summary>
        public int TripId { get; }

        /// <summary>
        /// See #/definitions/Car
        /// </summary>
        /// <value>See &#39;#/definitions/Car&#39;</value>
        public int CarId { get; }

        /// <summary>
        /// See &#39;#/definitions/Customer&#39;
        /// </summary>
        /// <value>See &#39;#/definitions/Customer&#39;</value>
        public int CustomerId { get; }

        /// <summary>
        /// Date and time when the trip started
        /// </summary>
        /// <value>Date and time when the trip started</value>
        public DateTime StartDate { get; }

        /// <summary>
        /// Date and time when the trip ended
        /// </summary>
        /// <value>Date and time when the trip ended</value>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or Sets StartPositionLatitude
        /// </summary>
        public double StartPositionLatitude { get; }

        /// <summary>
        /// Gets or Sets StartPositionLongitude
        /// </summary>
        public double StartPositionLongitude { get; }

        /// <summary>
        /// Gets or Sets EndPositionLatitude
        /// </summary>
        public double? EndPositionLatitude { get; set; }

        /// <summary>
        /// Gets or Sets EndPositionLongitude
        /// </summary>
        public double? EndPositionLongitude { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Trip {\n");
            sb.Append("  TripId: ").Append(TripId).Append("\n");
            sb.Append("  CarId: ").Append(CarId).Append("\n");
            sb.Append("  CustomerId: ").Append(CustomerId).Append("\n");
            sb.Append("  StartDate: ").Append(StartDate).Append("\n");
            sb.Append("  EndDate: ").Append(EndDate).Append("\n");
            sb.Append("  StartPositionLatitude: ").Append(StartPositionLatitude).Append("\n");
            sb.Append("  StartPositionLongitude: ").Append(StartPositionLongitude).Append("\n");
            sb.Append("  EndPositionLatitude: ").Append(EndPositionLatitude).Append("\n");
            sb.Append("  EndPositionLongitude: ").Append(EndPositionLongitude).Append("\n");
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
            return Equals((Trip)obj);
        }

        /// <summary>
        /// Returns true if Trip instances are equal
        /// </summary>
        /// <param name="other">Instance of Trip to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Trip other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    TripId == other.TripId ||
                    TripId.Equals(other.TripId)
                ) &&
                (
                    CarId == other.CarId ||
                    CarId.Equals(other.CarId)
                ) &&
                (
                    CustomerId == other.CustomerId ||
                    CustomerId.Equals(other.CustomerId)
                ) &&
                (
                    StartDate == other.StartDate ||
                    StartDate.Equals(other.StartDate)
                ) &&
                (
                    EndDate == other.EndDate ||
                    EndDate != null &&
                    EndDate.Equals(other.EndDate)
                ) &&
                (
                    Math.Abs(StartPositionLatitude - other.StartPositionLatitude) < 0.0001 ||
                    StartPositionLatitude.Equals(other.StartPositionLatitude)
                ) &&
                (
                    Math.Abs(StartPositionLongitude - other.StartPositionLongitude) < 0.0001 ||
                    StartPositionLongitude.Equals(other.StartPositionLongitude)
                ) &&
                (
                    EndPositionLatitude.HasValue && other.EndPositionLatitude.HasValue &&
                    Math.Abs(EndPositionLatitude.Value - other.EndPositionLatitude.Value) < 0.0001 ||
                    EndPositionLatitude != null &&
                    EndPositionLatitude.Equals(other.EndPositionLatitude)
                ) &&
                (
                    EndPositionLongitude.HasValue && other.EndPositionLongitude.HasValue &&
                    Math.Abs(EndPositionLongitude.Value - other.EndPositionLongitude.Value) < 0.0001 ||
                    EndPositionLongitude != null &&
                    EndPositionLongitude.Equals(other.EndPositionLongitude)
                );
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

                hash = hash * 59 + TripId.GetHashCode();
                hash = hash * 59 + CarId.GetHashCode();
                hash = hash * 59 + CustomerId.GetHashCode();
                hash = hash * 59 + StartDate.GetHashCode();
                hash = hash * 59 + StartPositionLatitude.GetHashCode();
                hash = hash * 59 + StartPositionLongitude.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Trip left, Trip right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Trip left, Trip right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
