using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class CarMaintenance
        : IEquatable<CarMaintenance>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CarMaintenance" /> class.
        /// </summary>
        /// <param name="carMaintenanceId">CarMaintenanceId (required)</param>
        /// <param name="carId">See #/definitions/Car  (required)</param>
        /// <param name="maintenanceId">See #/definitions/Maintenance (required)</param>
        /// <param name="invoiceItemId">See #/definitions/InvoiceItem (required)</param>
        /// <param name="plannedDate">PlannedDate</param>
        /// <param name="completedDate">CompletedDate</param>
        public CarMaintenance(uint carMaintenanceId, uint carId, uint maintenanceId, uint? invoiceItemId,
            DateTime? plannedDate, DateTime? completedDate)
        {
            CarMaintenanceId = carMaintenanceId;
            CarId = carId;
            MaintenanceId = maintenanceId;
            InvoiceItemId = invoiceItemId;
            PlannedDate = plannedDate;
            CompletedDate = completedDate;
        }

        /// <summary>
        /// Gets or Sets CarMaintenanceId
        /// </summary>
        [Required, Range(0, uint.MaxValue)]
        public uint CarMaintenanceId { get; }

        /// <summary>
        /// See #/definitions/Car
        /// </summary>
        /// <value>See #/definitions/Car</value>
        [Required, Range(1, uint.MaxValue)]
        public uint CarId { get; }

        /// <summary>
        /// See #/definitions/Maintenance
        /// </summary>
        /// <value>See #/definitions/Maintenance</value>
        [Required, Range(1, uint.MaxValue)]
        public uint MaintenanceId { get; }

        /// <summary>
        /// See #/definitions/InvoiceItem
        /// </summary>
        /// <value>See #/definitions/InvoiceItem</value>
        [Range(1, uint.MaxValue)]
        public uint? InvoiceItemId { get; set; }

        /// <summary>
        /// Gets or Sets PlannedDate
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? PlannedDate { get; }

        /// <summary>
        /// Gets or Sets CompletedDate
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CarMaintenance {\n");
            sb.Append("  CarMaintenanceId: ").Append(CarMaintenanceId).Append("\n");
            sb.Append("  CarId: ").Append(CarId).Append("\n");
            sb.Append("  MaintenanceId: ").Append(MaintenanceId).Append("\n");
            sb.Append("  InvoiceItemId: ").Append(InvoiceItemId).Append("\n");
            sb.Append("  PlannedDate: ").Append(PlannedDate).Append("\n");
            sb.Append("  CompletedDate: ").Append(CompletedDate).Append("\n");
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
            return Equals((CarMaintenance)obj);
        }

        /// <summary>
        /// Returns true if CarMaintenance instances are equal
        /// </summary>
        /// <param name="other">Instance of CarMaintenance to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CarMaintenance other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (CarMaintenanceId == other.CarMaintenanceId || CarMaintenanceId.Equals(other.CarMaintenanceId)) &&
                (CarId == other.CarId || CarId.Equals(other.CarId)) &&
                (MaintenanceId == other.MaintenanceId || MaintenanceId.Equals(other.MaintenanceId)) &&
                (InvoiceItemId == other.InvoiceItemId || InvoiceItemId.Equals(other.InvoiceItemId)) &&
                (PlannedDate == other.PlannedDate || PlannedDate != null && PlannedDate.Equals(other.PlannedDate)) &&
                (
                    CompletedDate == other.CompletedDate ||
                    CompletedDate != null && CompletedDate.Equals(other.CompletedDate)
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

                hash = hash * 59 + CarMaintenanceId.GetHashCode();
                hash = hash * 59 + CarId.GetHashCode();
                hash = hash * 59 + MaintenanceId.GetHashCode();
                if (PlannedDate != null)
                    hash = hash * 59 + PlannedDate.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(CarMaintenance left, CarMaintenance right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CarMaintenance left, CarMaintenance right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
