using System;
using System.Text;
using Newtonsoft.Json;

namespace ecruise.Models
{
    public class CarMaintenance :  IEquatable<CarMaintenance>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CarMaintenance" /> class.
        /// </summary>
        /// <param name="carMaintenanceId">CarMaintenanceId.</param>
        /// <param name="carId">See &#39;#/definitions/Car&#39;.</param>
        /// <param name="maintenanceId">See &#39;#/definitions/Maintenance&#39;.</param>
        /// <param name="invoiceItemId">See &#39;#/definitions/InvoiceItem&#39;.</param>
        /// <param name="plannedDate">PlannedDate.</param>
        /// <param name="completedDate">CompletedDate.</param>
        public CarMaintenance(int? carMaintenanceId, int? carId, int? maintenanceId, int? invoiceItemId, DateTime? plannedDate, DateTime? completedDate)
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
        public int? CarMaintenanceId { get; set; }
        /// <summary>
        /// See &#39;#/definitions/Car&#39;
        /// </summary>
        /// <value>See &#39;#/definitions/Car&#39;</value>
        public int? CarId { get; set; }
        /// <summary>
        /// See &#39;#/definitions/Maintenance&#39;
        /// </summary>
        /// <value>See &#39;#/definitions/Maintenance&#39;</value>
        public int? MaintenanceId { get; set; }
        /// <summary>
        /// See &#39;#/definitions/InvoiceItem&#39;
        /// </summary>
        /// <value>See &#39;#/definitions/InvoiceItem&#39;</value>
        public int? InvoiceItemId { get; set; }
        /// <summary>
        /// Gets or Sets PlannedDate
        /// </summary>
        public DateTime? PlannedDate { get; set; }
        /// <summary>
        /// Gets or Sets CompletedDate
        /// </summary>
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
                (
                    CarMaintenanceId == other.CarMaintenanceId ||
                    CarMaintenanceId != null &&
                    CarMaintenanceId.Equals(other.CarMaintenanceId)
                ) && 
                (
                    CarId == other.CarId ||
                    CarId != null &&
                    CarId.Equals(other.CarId)
                ) && 
                (
                    MaintenanceId == other.MaintenanceId ||
                    MaintenanceId != null &&
                    MaintenanceId.Equals(other.MaintenanceId)
                ) && 
                (
                    InvoiceItemId == other.InvoiceItemId ||
                    InvoiceItemId != null &&
                    InvoiceItemId.Equals(other.InvoiceItemId)
                ) && 
                (
                    PlannedDate == other.PlannedDate ||
                    PlannedDate != null &&
                    PlannedDate.Equals(other.PlannedDate)
                ) && 
                (
                    CompletedDate == other.CompletedDate ||
                    CompletedDate != null &&
                    CompletedDate.Equals(other.CompletedDate)
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
                if (CarMaintenanceId != null)
                    hash = hash * 59 + CarMaintenanceId.GetHashCode();
                if (CarId != null)
                    hash = hash * 59 + CarId.GetHashCode();
                if (MaintenanceId != null)
                    hash = hash * 59 + MaintenanceId.GetHashCode();
                if (InvoiceItemId != null)
                    hash = hash * 59 + InvoiceItemId.GetHashCode();
                if (PlannedDate != null)
                    hash = hash * 59 + PlannedDate.GetHashCode();
                if (CompletedDate != null)
                    hash = hash * 59 + CompletedDate.GetHashCode();
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
