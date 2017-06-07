using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class ChargingStation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChargingStation" /> class.
        /// </summary>
        /// <param name="chargingStationId">ChargingStationId (required)</param>
        /// <param name="slots">Slots (required)</param>
        /// <param name="slotsOccupied">SlotsOccupied (required)</param>
        /// <param name="latitude">Latitude (required)</param>
        /// <param name="longitude">Latitude (required)</param>
        public ChargingStation(uint chargingStationId, uint slots, uint slotsOccupied, double latitude,
            double longitude)
        {
            ChargingStationId = chargingStationId;
            Slots = slots;
            SlotsOccupied = slotsOccupied;
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        ///     Gets or Sets a CustomerId
        /// </summary>
        [Required]
        [Range(0, uint.MaxValue)]
        public uint ChargingStationId { get; }

        [Required]
        [Range(1, uint.MaxValue)]
        public uint Slots { get; }

        [Required]
        [Range(0, uint.MaxValue)]
        public uint SlotsOccupied { get; set; }

        [Required]
        [Range(-90.0, 90.0)]
        public double Latitude { get; }

        [Required]
        [Range(-180.0, 180.0)]
        public double Longitude { get; }

        /// <summary>
        ///     Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ChargingStation {\n");
            sb.Append("  ChargingStationId: ").Append(ChargingStationId).Append("\n");
            sb.Append("  Slots: ").Append(Slots).Append("\n");
            sb.Append("  SlotsOccupied: ").Append(SlotsOccupied).Append("\n");
            sb.Append("  Latitude: ").Append(Latitude).Append("\n");
            sb.Append("  Longitude: ").Append(Longitude).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        ///     Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        ///     Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ChargingStation)obj);
        }

        /// <summary>
        ///     Returns true if Address instances are equal
        /// </summary>
        /// <param name="other">Instance of Address to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ChargingStation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (ChargingStationId == other.ChargingStationId || ChargingStationId.Equals(other.ChargingStationId)) &&
                (Slots == other.Slots || Slots.Equals(other.Slots)) &&
                (SlotsOccupied == other.SlotsOccupied || SlotsOccupied.Equals(other.SlotsOccupied)) &&
                (Math.Abs(Latitude - other.Latitude) < 0.000000001 || Latitude.Equals(other.Latitude)) &&
                (Math.Abs(Longitude - other.Longitude) < 0.000000001 || Longitude.Equals(other.Longitude));
        }

        /// <summary>
        ///     Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 41;

                hash = hash * 59 + ChargingStationId.GetHashCode();
                hash = hash * 59 + Slots.GetHashCode();
                hash = hash * 59 + Latitude.GetHashCode();
                hash = hash * 59 + Longitude.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(ChargingStation left, ChargingStation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ChargingStation left, ChargingStation right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
