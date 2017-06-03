using System.Collections.Generic;
using System.Linq;

using CarChargingStation = ecruise.Models.CarChargingStation;
using DbCarChargingStation = ecruise.Database.Models.CarChargingStation;

namespace ecruise.Models.Assemblers
{
    public class CarChargingStationAssembler
    {
        public static DbCarChargingStation AssembleEntity(CarChargingStation carChargingStationModel)
        {
            return new DbCarChargingStation
            {
                CarChargingStationId = carChargingStationModel.CarChargingStationId,
                CarId = carChargingStationModel.CarId,
                ChargeEnd = carChargingStationModel.ChargeEnd,
                ChargeStart = carChargingStationModel.ChargeStart,
                ChargingStationId = carChargingStationModel.CarChargingStationId
            };
        }

        public static CarChargingStation AssembleModel(DbCarChargingStation carChargingStationEntity)
        {
            return new CarChargingStation(
                carChargingStationEntity.CarChargingStationId,
                carChargingStationEntity.CarId,
                carChargingStationEntity.ChargingStationId,
                carChargingStationEntity.ChargeStart,
                carChargingStationEntity.ChargeEnd
            );
        }

        public static List<CarChargingStation> AssembleModelList(IList<DbCarChargingStation> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbCarChargingStation> AssembleEntityList(IList<CarChargingStation> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}

