using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class Car : IEquatable<Car>
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
        /// Gets or Sets ChargingState
        /// </summary>
        public ChargingStateEnum? ChargingState { get; set; }

        /// <summary>
        /// Gets or Sets BookingState
        /// </summary>
        public BookingStateEnum? BookingState { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Car" /> class.
        /// </summary>
        /// <param name="carId">CarId.</param>
        /// <param name="licensePlate">LicensePlate.</param>
        /// <param name="chargingState">ChargingState.</param>
        /// <param name="bookingState">BookingState.</param>
        /// <param name="mileage">Mileage.</param>
        /// <param name="chargeLevel">Current charging level of the car. From 0. to 100..</param>
        /// <param name="kilowatts">Kilowatts.</param>
        /// <param name="manufacturer">Manufacturer.</param>
        /// <param name="model">Model.</param>
        /// <param name="yearOfConstruction">YearOfConstruction.</param>
        /// <param name="lastKnownPositionLatitude">LastKnownPositionLatitude.</param>
        /// <param name="lastKnownPositionLongitude">LastKnownPositionLongitude.</param>
        /// <param name="lastKnownPositionDate">LastKnownPositionDate.</param>
        public Car(int carId, string licensePlate, ChargingStateEnum chargingState, BookingStateEnum bookingState,
            int mileage, double chargeLevel, int kilowatts, string manufacturer, string model,
            string yearOfConstruction, double? lastKnownPositionLatitude, double? lastKnownPositionLongitude,
            DateTime? lastKnownPositionDate)
        {
            if (carId == 0)
                throw new ArgumentNullException("CarId is a required property for Car and cannot be zero");

            CarId = carId;
            LicensePlate = licensePlate ??
                           throw new ArgumentNullException(
                               "LicensePlate is a required property for Car and cannot be null");
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
        public int CarId { get; set; }

        /// <summary>
        /// Gets or Sets LicensePlate
        /// </summary>
        public string LicensePlate { get; set; }

        /// <summary>
        /// Gets or Sets Mileage
        /// </summary>
        public int Mileage { get; set; }

        /// <summary>
        /// Current charging level of the car. From 0. to 100.
        /// </summary>
        /// <value>Current charging level of the car. From 0. to 100.</value>
        public double ChargeLevel { get; set; }

        /// <summary>
        /// Gets or Sets Kilowatts
        /// </summary>
        public int Kilowatts { get; set; }

        /// <summary>
        /// Gets or Sets Manufacturer
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or Sets Model
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or Sets YearOfConstruction
        /// </summary>
        public string YearOfConstruction { get; set; }

        /// <summary>
        /// Gets or Sets LastKnownPositionLatitude
        /// </summary>
        public double? LastKnownPositionLatitude { get; set; }

        /// <summary>
        /// Gets or Sets LastKnownPositionLongitude
        /// </summary>
        public double? LastKnownPositionLongitude { get; set; }

        /// <summary>
        /// Gets or Sets LastKnownPositionDate
        /// </summary>
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
                (
                    CarId == other.CarId ||
                    CarId.Equals(other.CarId)
                ) &&
                (
                    LicensePlate == other.LicensePlate ||
                    LicensePlate != null &&
                    LicensePlate.Equals(other.LicensePlate)
                ) &&
                (
                    ChargingState == other.ChargingState ||
                    ChargingState != null &&
                    ChargingState.Equals(other.ChargingState)
                ) &&
                (
                    BookingState == other.BookingState ||
                    BookingState != null &&
                    BookingState.Equals(other.BookingState)
                ) &&
                (
                    Mileage == other.Mileage ||
                    Mileage.Equals(other.Mileage)
                ) &&
                (
                    Math.Abs(ChargeLevel - other.ChargeLevel) < 0.00001 ||
                    ChargeLevel.Equals(other.ChargeLevel)
                ) &&
                (
                    Kilowatts == other.Kilowatts ||
                    Kilowatts.Equals(other.Kilowatts)
                ) &&
                (
                    Manufacturer == other.Manufacturer ||
                    Manufacturer != null &&
                    Manufacturer.Equals(other.Manufacturer)
                ) &&
                (
                    Model == other.Model ||
                    Model != null &&
                    Model.Equals(other.Model)
                ) &&
                (
                    YearOfConstruction == other.YearOfConstruction ||
                    YearOfConstruction != null &&
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
                (
                    LastKnownPositionDate == other.LastKnownPositionDate ||
                    LastKnownPositionDate != null &&
                    LastKnownPositionDate.Equals(other.LastKnownPositionDate)
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
                if (CarId != null)
                    hash = hash * 59 + CarId.GetHashCode();
                if (LicensePlate != null)
                    hash = hash * 59 + LicensePlate.GetHashCode();
                if (ChargingState != null)
                    hash = hash * 59 + ChargingState.GetHashCode();
                if (BookingState != null)
                    hash = hash * 59 + BookingState.GetHashCode();
                if (Mileage != null)
                    hash = hash * 59 + Mileage.GetHashCode();
                if (ChargeLevel != null)
                    hash = hash * 59 + ChargeLevel.GetHashCode();
                if (Kilowatts != null)
                    hash = hash * 59 + Kilowatts.GetHashCode();
                if (Manufacturer != null)
                    hash = hash * 59 + Manufacturer.GetHashCode();
                if (Model != null)
                    hash = hash * 59 + Model.GetHashCode();
                if (YearOfConstruction != null)
                    hash = hash * 59 + YearOfConstruction.GetHashCode();
                if (LastKnownPositionLatitude != null)
                    hash = hash * 59 + LastKnownPositionLatitude.GetHashCode();
                if (LastKnownPositionLongitude != null)
                    hash = hash * 59 + LastKnownPositionLongitude.GetHashCode();
                if (LastKnownPositionDate != null)
                    hash = hash * 59 + LastKnownPositionDate.GetHashCode();
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
