using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Statistic
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Statistic" /> class.
        /// </summary>
        /// <param name="date">The date of the statistic</param>
        /// <param name="bookings">The number of bookings for the day</param>
        /// <param name="averageChargeLevel">The average charge level of all cars</param>
        /// <param name="carsInUse">The number of cars in use</param>
        /// <param name="carsCharging">The number of cars charging</param>
        public Statistic(DateTime date, uint bookings, double averageChargeLevel, uint carsInUse, uint carsCharging)
        {
            Date = date;
            Bookings = bookings;
            AverageChargeLevel = averageChargeLevel;
            CarsInUse = carsInUse;
            CarsCharging = carsCharging;
        }

        /// <summary>
        ///     Gets Date
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime Date { get; }

        /// <summary>
        ///     Gets Bookings
        /// </summary>
        [Required]
        [Range(0, uint.MaxValue)]
        public uint Bookings { get; }

        /// <summary>
        ///     Gets AverageChargeLevel
        /// </summary>
        [Required]
        [Range(0.0, 100.0)]
        public double AverageChargeLevel { get; }

        /// <summary>
        ///     Gets CarsInUse
        /// </summary>
        [Required]
        [Range(0, uint.MaxValue)]
        public uint CarsInUse { get; }

        /// <summary>
        ///     Gets CarsCharging
        /// </summary>
        [Required]
        [Range(0, uint.MaxValue)]
        public uint CarsCharging { get; }

        /// <summary>
        ///     Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Statistic {\n");
            sb.Append("  Date: ").Append(Date).Append("\n");
            sb.Append("  Bookings: ").Append(Bookings).Append("\n");
            sb.Append("  AverageChargeLevel: ").Append(AverageChargeLevel).Append("\n");
            sb.Append("  CarsInUse: ").Append(CarsInUse).Append("\n");
            sb.Append("  CarsCharging: ").Append(CarsCharging).Append("\n");
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
            return Equals((Statistic)obj);
        }

        /// <summary>
        ///     Returns true if Statistic instances are equal
        /// </summary>
        /// <param name="other">Instance of Statistic to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Statistic other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (Date == other.Date || Date.Equals(other.Date)) &&
                (Bookings == other.Bookings || Bookings.Equals(other.Bookings)) &&
                (Math.Abs(AverageChargeLevel - other.AverageChargeLevel) < 0.001 ||
                 AverageChargeLevel.Equals(other.AverageChargeLevel)) &&
                (CarsInUse == other.CarsInUse || CarsInUse.Equals(other.CarsInUse)) &&
                (CarsCharging == other.CarsCharging || CarsCharging.Equals(other.CarsCharging));
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

                hash = hash * 59 + Date.GetHashCode();
                hash = hash * 59 + Bookings.GetHashCode();
                hash = hash * 59 + AverageChargeLevel.GetHashCode();
                hash = hash * 59 + CarsInUse.GetHashCode();
                hash = hash * 59 + CarsCharging.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Statistic left, Statistic right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Statistic left, Statistic right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
