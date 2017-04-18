using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Maintenance :  IEquatable<Maintenance>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Maintenance" /> class.
        /// </summary>
        /// <param name="maintenenaceId">MaintenenaceId.</param>
        /// <param name="spontaneously">Spontaneously.</param>
        /// <param name="atMileage">AtMileage.</param>
        /// <param name="atDate">AtDate.</param>
        public Maintenance(int? maintenenaceId, bool? spontaneously, int? atMileage, DateTime? atDate)
        {
            MaintenenaceId = maintenenaceId;
            Spontaneously = spontaneously;
            AtMileage = atMileage;
            AtDate = atDate;            
        }

        /// <summary>
        /// Gets or Sets MaintenenaceId
        /// </summary>
        public int? MaintenenaceId { get; set; }
        /// <summary>
        /// Gets or Sets Spontaneously
        /// </summary>
        public bool? Spontaneously { get; set; }
        /// <summary>
        /// Gets or Sets AtMileage
        /// </summary>
        public int? AtMileage { get; set; }
        /// <summary>
        /// Gets or Sets AtDate
        /// </summary>
        public DateTime? AtDate { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Maintenance {\n");
            sb.Append("  MaintenenaceId: ").Append(MaintenenaceId).Append("\n");
            sb.Append("  Spontaneously: ").Append(Spontaneously).Append("\n");
            sb.Append("  AtMileage: ").Append(AtMileage).Append("\n");
            sb.Append("  AtDate: ").Append(AtDate).Append("\n");
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
            return Equals((Maintenance)obj);
        }

        /// <summary>
        /// Returns true if Maintenance instances are equal
        /// </summary>
        /// <param name="other">Instance of Maintenance to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Maintenance other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    MaintenenaceId == other.MaintenenaceId ||
                    MaintenenaceId != null &&
                    MaintenenaceId.Equals(other.MaintenenaceId)
                ) && 
                (
                    Spontaneously == other.Spontaneously ||
                    Spontaneously != null &&
                    Spontaneously.Equals(other.Spontaneously)
                ) && 
                (
                    AtMileage == other.AtMileage ||
                    AtMileage != null &&
                    AtMileage.Equals(other.AtMileage)
                ) && 
                (
                    AtDate == other.AtDate ||
                    AtDate != null &&
                    AtDate.Equals(other.AtDate)
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
                // Suitable nullity checks etc, of course :)
                    if (MaintenenaceId != null)
                    hash = hash * 59 + MaintenenaceId.GetHashCode();
                    if (Spontaneously != null)
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
