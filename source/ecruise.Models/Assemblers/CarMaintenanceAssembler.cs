using System.Collections.Generic;
using System.Linq;

using CarMaintenance = ecruise.Models.CarMaintenance;
using DbCarMaintenance = ecruise.Database.Models.CarMaintenance;

namespace ecruise.Models.Assemblers
{
    public class CarMaintenanceAssembler
    {
        public static DbCarMaintenance AssembleEntity(ulong carMaintenanceId, CarMaintenance carMaintenanceModel)
        {
            return new DbCarMaintenance
            {
                CarId = carMaintenanceId != 0 ? carMaintenanceId : carMaintenanceModel.CarId,
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
                (uint)carMaintenanceEntity.CarMaintenanceId,
                (uint)carMaintenanceEntity.CarId,
                (uint)carMaintenanceEntity.MaintenanceId,
                (uint?)carMaintenanceEntity.InvoiceItemId,
                carMaintenanceEntity.PlannedDate,
                carMaintenanceEntity.CompletedDate
            );
        }

        public static List<CarMaintenance> AssembleModelList(IList<DbCarMaintenance> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbCarMaintenance> AssembleEntityList(bool setIdsNull, IList<CarMaintenance> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            else
                return models.Select(e => AssembleEntity(e.CarMaintenanceId, e)).ToList();
        }
    }
}
