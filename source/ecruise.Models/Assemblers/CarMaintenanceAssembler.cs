using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    class CarMaintenanceAssembler
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
                carMaintenanceEntity.CarMaintenanceId,
                carMaintenanceEntity.CarId,
                carMaintenanceEntity.MaintenanceId,
                carMaintenanceEntity.InvoiceItemId,
                carMaintenanceEntity.PlannedDate,
                carMaintenanceEntity.CompletedDate
            );
        }
    }
}
