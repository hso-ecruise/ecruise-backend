using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class CarChargingStation
        : IEquatable<CarChargingStation>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CarChargingStation" /> class.
        /// </summary>
        /// <param name="carChargingStationId">CarChargingStationId (required)</param>
        /// <param name="carId">CarId (required)</param>
        /// <param name="chargingStationId">ChargingStationId (required)</param>
        /// <param name="chargeStart">ChargeStart (required)</param>
        /// <param name="chargeEnd">ChargeEnd</param>
        public CarChargingStation(uint carChargingStationId, uint carId, uint chargingStationId,
            DateTime chargeStart, DateTime? chargeEnd)
        {
            CarChargingStationId = carChargingStationId;
            CarId = carId;
            ChargingStationId = chargingStationId;
            ChargeStart = chargeStart;
            ChargeEnd = chargeEnd;
        }

        /// <summary>
        ///     Gets or Sets CarChargingStationId
        /// </summary>
        [Required]
        [Range(0, uint.MaxValue)]
        public uint CarChargingStationId { get; }

        /// <summary>
        ///     Gets or Sets CarId
        /// </summary>
        [Required]
        [Range(1, uint.MaxValue)]
        public uint CarId { get; }

        /// <summary>
        ///     Gets or Sets ChargingStationId
        /// </summary>
        [Required]
        [Range(1, uint.MaxValue)]
        public uint ChargingStationId { get; }

        /// <summary>
        ///     Gets or Sets ChargeStart
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ChargeStart { get; }

        /// <summary>
        ///     Gets or Sets ChargeEnd
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? ChargeEnd { get; set; }

        /// <summary>
        ///     Returns true if CarChargingStation instances are equal
        /// </summary>
        /// <param name="other">Instance of CarChargingStation to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CarChargingStation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    CarChargingStationId == other.CarChargingStationId ||
                    CarChargingStationId.Equals(other.CarChargingStationId)
                ) &&
                (CarId == other.CarId || CarId.Equals(other.CarId)) &&
                (ChargingStationId == other.ChargingStationId || ChargingStationId.Equals(other.ChargingStationId)) &&
                (ChargeStart == other.ChargeStart || ChargeStart.Equals(other.ChargeStart)) &&
                (ChargeEnd == other.ChargeEnd || ChargeEnd != null && ChargeEnd.Equals(other.ChargeEnd));
        }

        /// <summary>
        ///     Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CarChargingStation {\n");
            sb.Append("  CarChargingStationId: ").Append(CarChargingStationId).Append("\n");
            sb.Append("  CarId: ").Append(CarId).Append("\n");
            sb.Append("  ChargingStationId: ").Append(ChargingStationId).Append("\n");
            sb.Append("  ChargeStart: ").Append(ChargeStart).Append("\n");
            sb.Append("  ChargeEnd: ").Append(ChargeEnd).Append("\n");
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
            return Equals((CarChargingStation)obj);
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

                hash = hash * 59 + CarChargingStationId.GetHashCode();
                hash = hash * 59 + CarId.GetHashCode();
                hash = hash * 59 + ChargingStationId.GetHashCode();
                hash = hash * 59 + ChargeStart.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(CarChargingStation left, CarChargingStation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CarChargingStation left, CarChargingStation right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
