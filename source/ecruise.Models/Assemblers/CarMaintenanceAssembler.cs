using System.Collections.Generic;
using System.Linq;

using CarMaintenance = ecruise.Models.CarMaintenance;
using DbCarMaintenance = ecruise.Database.Models.CarMaintenance;

namespace ecruise.Models.Assemblers
{
    public class CarMaintenanceAssembler
    {
        public static DbCarMaintenance AssembleEntity(CarMaintenance carMaintenanceModel)
        {
            return new DbCarMaintenance
            {
                CarId = carMaintenanceModel.CarId,
                CarMaintenanceId = carMaintenanceModel.CarMaintenanceId,
                CompletedDate = carMaintenanceModel.CompletedDate,
                InvoiceItemId = carMaintenanceModel.InvoiceItemId,
                MaintenanceId = carMaintenanceModel.MaintenanceId,
                PlannedDate = carMaintenanceModel.PlannedDate
            };
        }

        public static CarMaintenance AssembleModel(DbCarMaintenance carMaintenanceEntity)
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

        public static List<CarMaintenance> AssembleModelList(IList<DbCarMaintenance> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbCarMaintenance> AssembleEntityList(IList<CarMaintenance> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}
