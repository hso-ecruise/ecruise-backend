using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    class MaintenanceAssembler
    {
        public static Database.Models.Maintenance AssembleEntity(Maintenance maintenanceModel)
        {
            return new Database.Models.Maintenance
            {
                MaintenanceId = maintenanceModel.MaintenenaceId,
                Spontaneously = maintenanceModel.Spontaneously,
                AtMileage = maintenanceModel.AtMileage,
                AtDate = maintenanceModel.AtDate
            };
        }

        public static Maintenance AssembleModel(Database.Models.Maintenance maintenanceEntity)
        {
            return new Maintenance(
                maintenanceEntity.MaintenanceId,
                maintenanceEntity.Spontaneously,
                maintenanceEntity.AtMileage,
                maintenanceEntity.AtDate);
        }
    }
}
