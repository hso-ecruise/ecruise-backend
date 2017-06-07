using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Maintenance
        : IEquatable<Maintenance>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Maintenance" /> class.
        /// </summary>
        /// <param name="maintenenaceId">MaintenanceId (required)</param>
        /// <param name="spontaneously">Spontaneously (required)</param>
        /// <param name="atMileage">AtMileage</param>
        /// <param name="atDate">AtDate</param>
        public Maintenance(uint maintenenaceId, bool spontaneously, uint? atMileage, DateTime? atDate)
        {
            if (spontaneously && (atMileage.HasValue || atDate.HasValue))
                throw new ArgumentException("Neither " + nameof(atMileage) + " nor " + nameof(atDate) +
                                            " can have a value if the Maintenance is spontaneous.");
            if (!spontaneously && !atMileage.HasValue && !atDate.HasValue)
                throw new ArgumentException("Either " + nameof(atMileage) + " or " + nameof(atDate) +
                                            " is required to have a value if the Maintenance is planned");

            MaintenanceId = maintenenaceId;
            Spontaneously = spontaneously;
            AtMileage = atMileage;
            AtDate = atDate;
        }

        /// <summary>
        ///     Gets MaintenenaceId
        /// </summary>
        [Required]
        [Range(0, uint.MaxValue)]
        public uint MaintenanceId { get; }

        /// <summary>
        ///     Gets Spontaneously
        /// </summary>
        [Required]
        public bool Spontaneously { get; }

        /// <summary>
        ///     Gets AtMileage
        /// </summary>
        [Range(0, uint.MaxValue)]
        public uint? AtMileage { get; }

        /// <summary>
        ///     Gets AtDate
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? AtDate { get; }

        /// <summary>
        ///     Returns true if Maintenance instances are equal
        /// </summary>
        /// <param name="other">Instance of Maintenance to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Maintenance other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (MaintenanceId == other.MaintenanceId || MaintenanceId.Equals(other.MaintenanceId)) &&
                (Spontaneously == other.Spontaneously || Spontaneously.Equals(other.Spontaneously)) &&
                (AtMileage == other.AtMileage || AtMileage != null && AtMileage.Equals(other.AtMileage)) &&
                (AtDate == other.AtDate || AtDate != null && AtDate.Equals(other.AtDate));
        }

        /// <summary>
        ///     Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Maintenance {\n");
            sb.Append("  MaintenanceId: ").Append(MaintenanceId).Append("\n");
            sb.Append("  Spontaneously: ").Append(Spontaneously).Append("\n");
            sb.Append("  AtMileage: ").Append(AtMileage).Append("\n");
            sb.Append("  AtDate: ").Append(AtDate).Append("\n");
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
            return Equals((Maintenance)obj);
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

                hash = hash * 59 + MaintenanceId.GetHashCode();
                hash = hash * 59 + Spontaneously.GetHashCode();
                if (AtMileage != null)
                    hash = hash * 59 + AtMileage.GetHashCode();
                if (AtDate != null)
                    hash = hash * 59 + AtDate.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Maintenance left, Maintenance right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Maintenance left, Maintenance right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
