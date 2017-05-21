using System;
using System.ComponentModel.DataAnnotations;
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
        /// <param name="tripId">TripId (required)</param>
        /// <param name="carId">See #/definitions/Car</param>
        /// <param name="customerId">See #/definitions/Customer (required)</param>
        /// <param name="startDate">Date and time when the trip started (required)</param>
        /// <param name="endDate">Date and time when the trip ended</param>
        /// <param name="startChargingStationId">StartChargingStationId</param>
        /// <param name="endChargingStationId">EndChargingStationId</param>
        /// <param name="distanceTravelled">DistanceTravelled</param>
        public Trip(uint tripId, uint? carId, uint customerId, DateTime? startDate, DateTime? endDate,
            uint? startChargingStationId, uint? endChargingStationId, double? distanceTravelled)
        {
            TripId = tripId;
            CarId = carId;
            CustomerId = customerId;
            StartDate = startDate;
            EndDate = endDate;
            StartChargingStationId = startChargingStationId;
            EndChargingStationId = endChargingStationId;
            DistanceTravelled = distanceTravelled;
        }

        /// <summary>
        /// Gets TripId
        /// </summary>
        [Range(1, uint.MaxValue)]
        public uint TripId { get; }

        /// <summary>
        /// See #/definitions/Car
        /// </summary>
        /// <value>See '#/definitions/Car'</value>
        [Range(1, uint.MaxValue)]
        public uint? CarId { get; set; }

        /// <summary>
        /// See '#/definitions/Customer'
        /// </summary>
        /// <value>See '#/definitions/Customer'</value>
        [Required, Range(1, uint.MaxValue)]
        public uint CustomerId { get; }

        /// <summary>
        /// Date and time when the trip started
        /// </summary>
        /// <value>Date and time when the trip started</value>
        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; }

        /// <summary>
        /// Date and time when the trip ended
        /// </summary>
        /// <value>Date and time when the trip ended</value>
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets StartPositionLatitude
        /// </summary>
        [Range(1, uint.MaxValue)]
        public uint? StartChargingStationId { get; set; }

        /// <summary>
        /// Gets StartPositionLongitude
        /// </summary>
        [Range(1, uint.MaxValue)]
        public uint? EndChargingStationId { get; set; }

        /// <summary>
        /// Gets DistanceTravelled
        /// </summary>
        [Range(0, double.MaxValue)]
        public double? DistanceTravelled { get; set; }

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
            sb.Append("  StartChargingStationId: ").Append(StartChargingStationId).Append("\n");
            sb.Append("  EndChargingStationId: ").Append(EndChargingStationId).Append("\n");
            sb.Append("  DistanceTravelled: ").Append(DistanceTravelled).Append("\n");
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
                (TripId == other.TripId || TripId.Equals(other.TripId)) &&
                (CarId == other.CarId || CarId.Equals(other.CarId)) &&
                (CustomerId == other.CustomerId || CustomerId.Equals(other.CustomerId)) &&
                (StartDate == other.StartDate || StartDate.Equals(other.StartDate)) &&
                (EndDate == other.EndDate || EndDate != null && EndDate.Equals(other.EndDate)) &&
                (StartChargingStationId == other.StartChargingStationId ||
                 StartChargingStationId.Equals(other.StartChargingStationId)
                ) &&
                (EndChargingStationId == other.EndChargingStationId ||
                 EndChargingStationId.Equals(other.EndChargingStationId)
                ) &&
                ((DistanceTravelled.HasValue && other.DistanceTravelled.HasValue) &&
                 (Math.Abs(DistanceTravelled.Value - other.DistanceTravelled.Value) < 0.001) ||
                 DistanceTravelled.Equals(other.DistanceTravelled)
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
                hash = hash * 59 + CustomerId.GetHashCode();
                hash = hash * 59 + StartDate.GetHashCode();

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

    public class TripUpdate
        : IEquatable<TripUpdate>
    {
        /// <param name="endChargingStationId">EndChargingStationId</param>
        /// <param name="distanceTravelled">DistanceTravelled</param>
        public TripUpdate(uint endChargingStationId, double distanceTravelled)
        {
            EndChargingStationId = endChargingStationId;
            DistanceTravelled = distanceTravelled;
        }

        /// <summary>
        /// Gets StartPositionLongitude
        /// </summary>
        [Required, Range(1, uint.MaxValue)]
        public uint EndChargingStationId { get; }

        /// <summary>
        /// Gets DistanceTravelled
        /// </summary>
        [Required, Range(0, double.MaxValue)]
        public double DistanceTravelled { get; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class TripUpdate {\n");
            sb.Append("  EndChargingStationId: ").Append(EndChargingStationId).Append("\n");
            sb.Append("  DistanceTravelled: ").Append(DistanceTravelled).Append("\n");
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
        public bool Equals(TripUpdate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    EndChargingStationId == other.EndChargingStationId ||
                    EndChargingStationId.Equals(other.EndChargingStationId)
                ) &&
                (
                    Math.Abs(DistanceTravelled - other.DistanceTravelled) < 0.001 ||
                    DistanceTravelled.Equals(other.DistanceTravelled)
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

                hash = hash * 59 + EndChargingStationId.GetHashCode();
                hash = hash * 59 + DistanceTravelled.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(TripUpdate left, TripUpdate right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TripUpdate left, TripUpdate right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
