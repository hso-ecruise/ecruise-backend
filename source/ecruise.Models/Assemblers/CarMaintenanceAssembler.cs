using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    public class CarMaintenanceAssembler
    {
        public static Database.Models.CarMaintenance AssembleEntity(CarMaintenance carMaintenanceModel)
        {
            return new Database.Models.CarMaintenance
            {
                CarId = carMaintenanceModel.CarId,
                CarMaintenanceId = carMaintenanceModel.CarMaintenanceId,
                CompletedDate = carMaintenanceModel.CompletedDate,
                InvoiceItemId = carMaintenanceModel.InvoiceItemId,
                MaintenanceId = carMaintenanceModel.MaintenanceId,
                PlannedDate = carMaintenanceModel.PlannedDate
            };
        }

        public static CarMaintenance AssembleModel(Database.Models.CarMaintenance carMaintenanceEntity)
        {
            return new CarMaintenance(
                (uint)carMaintenanceEntity.CarMaintenanceId,
                (uint)carMaintenanceEntity.CarId,
                (uint)carMaintenanceEntity.MaintenanceId,
                (uint?)carMaintenanceEntity.InvoiceItemId,
                carMaintenanceEntity.PlannedDate,
                carMaintenanceEntity.CompletedDate
            );
        }
    }
}
