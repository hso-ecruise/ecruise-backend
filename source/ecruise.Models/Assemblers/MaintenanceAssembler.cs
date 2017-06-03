using System.Collections.Generic;
using System.Linq;
using Maintenance = ecruise.Models.Maintenance;
using DbMaintenance = ecruise.Database.Models.Maintenance;

namespace ecruise.Models.Assemblers
{
    public class MaintenanceAssembler
    {
        public static DbMaintenance AssembleEntity(ulong maintenanceId, Maintenance maintenanceModel)
        {
            return new DbMaintenance
            {
                MaintenanceId = maintenanceId != 0 ? maintenanceId : maintenanceModel.MaintenenaceId,
                Spontaneously = maintenanceModel.Spontaneously,
                AtMileage = maintenanceModel.AtMileage,
                AtDate = maintenanceModel.AtDate
            };
        }

        public static Maintenance AssembleModel(DbMaintenance maintenanceEntity)
        {
            return new Maintenance(
                (uint)maintenanceEntity.MaintenanceId,
                maintenanceEntity.Spontaneously,
                maintenanceEntity.AtMileage,
                maintenanceEntity.AtDate);
        }
        public static List<Maintenance> AssembleModelList(IList<DbMaintenance> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbMaintenance> AssembleEntityList(bool setIdsNull, IList<Maintenance> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            else
                return models.Select(e => AssembleEntity(e.MaintenenaceId, e)).ToList();
        }
    }
}
