using System.Collections.Generic;
using System.Linq;
using Maintenance = ecruise.Models.Maintenance;
using DbMaintenance = ecruise.Database.Models.Maintenance;

namespace ecruise.Models.Assemblers
{
    public class MaintenanceAssembler
    {
        public static DbMaintenance AssembleEntity(Maintenance maintenanceModel)
        {
            return new DbMaintenance
            {
                MaintenanceId = maintenanceModel.MaintenenaceId,
                Spontaneously = maintenanceModel.Spontaneously,
                AtMileage = maintenanceModel.AtMileage,
                AtDate = maintenanceModel.AtDate
            };
        }

        public static Maintenance AssembleModel(DbMaintenance maintenanceEntity)
        {
            return new Maintenance(
                maintenanceEntity.MaintenanceId,
                maintenanceEntity.Spontaneously,
                maintenanceEntity.AtMileage,
                maintenanceEntity.AtDate);
        }
        public static List<Maintenance> AssembleModelList(IList<DbMaintenance> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbMaintenance> AssembleEntityList(IList<Maintenance> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}
