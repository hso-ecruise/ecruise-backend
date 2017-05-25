using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Car
        : IEquatable<Car>
    {
        /// <summary>
        /// Gets or Sets ChargingState
        /// </summary>
        public enum ChargingStateEnum
        {
            Discharging,
            Charging,
            Full
        }

        /// <summary>
        /// Gets or Sets BookingState
        /// </summary>
        public enum BookingStateEnum
        {
            Available,
            Booked,
            Blocked
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Car" /> class.
        /// </summary>
        /// <param name="carId">CarId (required)</param>
        /// <param name="licensePlate">LicensePlate (required)</param>
        /// <param name="chargingState">ChargingState (required)</param>
        /// <param name="bookingState">BookingState (required)</param>
        /// <param name="mileage">Mileage (required)</param>
        /// <param name="chargeLevel">Current charging level of the car. From 0. to 100. (required)</param>
        /// <param name="kilowatts">Kilowatts (required)</param>
        /// <param name="manufacturer">Manufacturer (required)</param>
        /// <param name="model">Model (required)</param>
        /// <param name="yearOfConstruction">YearOfConstruction (required)</param>
        /// <param name="lastKnownPositionLatitude">LastKnownPositionLatitude</param>
        /// <param name="lastKnownPositionLongitude">LastKnownPositionLongitude</param>
        /// <param name="lastKnownPositionDate">LastKnownPositionDate</param>
        /// <exception cref="ArgumentNullException">The described argument has an invalid value.</exception>
        public Car(uint carId, string licensePlate, ChargingStateEnum chargingState, BookingStateEnum bookingState,
            uint mileage, double chargeLevel, uint kilowatts, string manufacturer, string model,
            int yearOfConstruction, double? lastKnownPositionLatitude, double? lastKnownPositionLongitude,
            DateTime? lastKnownPositionDate)
        {
            CarId = carId;
            LicensePlate = licensePlate;
            ChargingState = chargingState;
            BookingState = bookingState;
            Mileage = mileage;
            ChargeLevel = chargeLevel;
            Kilowatts = kilowatts;
            Manufacturer = manufacturer;
            Model = model;
            YearOfConstruction = yearOfConstruction;
            LastKnownPositionLatitude = lastKnownPositionLatitude;
            LastKnownPositionLongitude = lastKnownPositionLongitude;
            LastKnownPositionDate = lastKnownPositionDate;
        }

        /// <summary>
        /// Gets or Sets CarId
        /// </summary>
        [Required, Range(0, uint.MaxValue)]
        public uint CarId { get; }

        /// <summary>
        /// Gets or Sets LicensePlate
        /// </summary>
        [Required, RegularExpression(@"^[A-Z]{1,3} [A-Z]{1,2} [0-9]{1,4}$"), StringLength(16, MinimumLength = 5)]
        public string LicensePlate { get; }

        /// <summary>
        /// Gets or Sets ChargingState
        /// </summary>
        [Required]
        public ChargingStateEnum ChargingState { get; set; }

        /// <summary>
        /// Gets or Sets BookingState
        /// </summary>
        [Required]
        public BookingStateEnum BookingState { get; set; }

        /// <summary>
        /// Gets or Sets Mileage
        /// </summary>
        [Required, Range(0, uint.MaxValue)]
        public uint Mileage { get; set; }

        /// <summary>
        /// Current charging level of the car. From 0. to 100.
        /// </summary>
        /// <value>Current charging level of the car. From 0. to 100.</value>
        [Required, Range(0.0, 100.0)]
        public double ChargeLevel { get; set; }

        /// <summary>
        /// Gets or Sets Kilowatts
        /// </summary>
        [Required, Range(0, 999)]
        public uint Kilowatts { get; }

        /// <summary>
        /// Gets or Sets Manufacturer
        /// </summary>
        [Required, StringLength(64)]
        public string Manufacturer { get; }

        /// <summary>
        /// Gets or Sets Model
        /// </summary>
        [Required, StringLength(32)]
        public string Model { get; }

        /// <summary>
        /// Gets or Sets YearOfConstruction
        /// </summary>
        [Required, Range(1950, 2100)]
        public int YearOfConstruction { get; }

        /// <summary>
        /// Gets or Sets LastKnownPositionLatitude
        /// </summary>
        [Range(-90.0, 90.0)]
        public double? LastKnownPositionLatitude { get; set; }

        /// <summary>
        /// Gets or Sets LastKnownPositionLongitude
        /// </summary>
        [Range(-180.0, 180.0)]
        public double? LastKnownPositionLongitude { get; set; }

        /// <summary>
        /// Gets or Sets LastKnownPositionDate
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? LastKnownPositionDate { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Car {\n");
            sb.Append("  CarId: ").Append(CarId).Append("\n");
            sb.Append("  LicensePlate: ").Append(LicensePlate).Append("\n");
            sb.Append("  ChargingState: ").Append(ChargingState).Append("\n");
            sb.Append("  BookingState: ").Append(BookingState).Append("\n");
            sb.Append("  Mileage: ").Append(Mileage).Append("\n");
            sb.Append("  ChargeLevel: ").Append(ChargeLevel).Append("\n");
            sb.Append("  Kilowatts: ").Append(Kilowatts).Append("\n");
            sb.Append("  Manufacturer: ").Append(Manufacturer).Append("\n");
            sb.Append("  Model: ").Append(Model).Append("\n");
            sb.Append("  YearOfConstruction: ").Append(YearOfConstruction).Append("\n");
            sb.Append("  LastKnownPositionLatitude: ").Append(LastKnownPositionLatitude).Append("\n");
            sb.Append("  LastKnownPositionLongitude: ").Append(LastKnownPositionLongitude).Append("\n");
            sb.Append("  LastKnownPositionDate: ").Append(LastKnownPositionDate).Append("\n");
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
            return Equals((Car)obj);
        }

        /// <summary>
        /// Returns true if Car instances are equal
        /// </summary>
        /// <param name="other">Instance of Car to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Car other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (CarId == other.CarId || CarId.Equals(other.CarId)) &&
                (LicensePlate == other.LicensePlate || LicensePlate.Equals(other.LicensePlate)) &&
                (ChargingState == other.ChargingState || ChargingState.Equals(other.ChargingState)) &&
                (BookingState == other.BookingState || BookingState.Equals(other.BookingState)) &&
                (Mileage == other.Mileage || Mileage.Equals(other.Mileage)) &&
                (Math.Abs(ChargeLevel - other.ChargeLevel) < 0.00001 || ChargeLevel.Equals(other.ChargeLevel)) &&
                (Kilowatts == other.Kilowatts || Kilowatts.Equals(other.Kilowatts)) &&
                (Manufacturer == other.Manufacturer || Manufacturer.Equals(other.Manufacturer)) &&
                (Model == other.Model || Model.Equals(other.Model)) &&
                (
                    YearOfConstruction == other.YearOfConstruction ||
                    YearOfConstruction.Equals(other.YearOfConstruction)
                ) &&
                (
                    LastKnownPositionLatitude.HasValue && other.LastKnownPositionLatitude.HasValue &&
                    Math.Abs(LastKnownPositionLatitude.Value - other.LastKnownPositionLatitude.Value) < 0.0001 ||
                    LastKnownPositionLatitude != null &&
                    LastKnownPositionLatitude.Equals(other.LastKnownPositionLatitude)
                ) &&
                (
                    LastKnownPositionLongitude.HasValue && other.LastKnownPositionLongitude.HasValue &&
                    Math.Abs(LastKnownPositionLongitude.Value - other.LastKnownPositionLongitude.Value) < 0.001 ||
                    LastKnownPositionLongitude != null &&
                    LastKnownPositionLongitude.Equals(other.LastKnownPositionLongitude)
                ) &&
                (LastKnownPositionDate == other.LastKnownPositionDate ||
                 LastKnownPositionDate != null && LastKnownPositionDate.Equals(other.LastKnownPositionDate)
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

                hash = hash * 59 + CarId.GetHashCode();
                hash = hash * 59 + LicensePlate.GetHashCode();
                hash = hash * 59 + Kilowatts.GetHashCode();
                hash = hash * 59 + Manufacturer.GetHashCode();
                hash = hash * 59 + Model.GetHashCode();
                hash = hash * 59 + YearOfConstruction.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Car left, Car right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Car left, Car right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
