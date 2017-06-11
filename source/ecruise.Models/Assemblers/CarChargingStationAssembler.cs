using System.Collections.Generic;
using System.Linq;
using DbCarChargingStation = ecruise.Database.Models.CarChargingStation;

namespace ecruise.Models.Assemblers
{
    public static class CarChargingStationAssembler
    {
        public static DbCarChargingStation AssembleEntity(ulong carChargingStationId,
            CarChargingStation carChargingStationModel)
        {
            return new DbCarChargingStation
            {
                CarChargingStationId = carChargingStationId != 0
                    ? carChargingStationId
                    : carChargingStationModel.CarChargingStationId,
                CarId = carChargingStationModel.CarId,
                ChargeEnd = carChargingStationModel.ChargeEnd,
                ChargeStart = carChargingStationModel.ChargeStart,
                ChargingStationId = carChargingStationModel.ChargingStationId
            };
        }

        public static CarChargingStation AssembleModel(DbCarChargingStation carChargingStationEntity)
        {
            return new CarChargingStation(
                (uint)carChargingStationEntity.CarChargingStationId,
                (uint)carChargingStationEntity.CarId,
                (uint)carChargingStationEntity.ChargingStationId,
                carChargingStationEntity.ChargeStart,
                carChargingStationEntity.ChargeEnd
            );
        }

        public static List<CarChargingStation> AssembleModelList(IList<DbCarChargingStation> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbCarChargingStation> AssembleEntityList(bool setIdsNull, IList<CarChargingStation> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            return models.Select(e => AssembleEntity(e.CarChargingStationId, e)).ToList();
        }
    }
}
