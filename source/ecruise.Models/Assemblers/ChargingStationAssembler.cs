using System.Collections.Generic;
using System.Linq;

using ChargingStation = ecruise.Models.ChargingStation;
using DbChargingStation = ecruise.Database.Models.ChargingStation;

namespace ecruise.Models.Assemblers
{
    public static class ChargingStationAssembler
    {
        public static DbChargingStation AssembleEntity(ulong chargingStationId, ChargingStation chargingStationModel)
        {
            return new DbChargingStation
            {
                ChargingStationId = chargingStationId != 0 ? chargingStationId : chargingStationModel.ChargingStationId,
                Slots = chargingStationModel.Slots,
                SlotsOccupied = chargingStationModel.SlotsOccupied,
                Latitude = chargingStationModel.Latitude,
                Longitude = chargingStationModel.Longitude
            };
        }
        public static ChargingStation AssembleModel(DbChargingStation chargingStationEntity)

        {
            return new ChargingStation(
                (uint)chargingStationEntity.ChargingStationId,
                chargingStationEntity.Slots,
                chargingStationEntity.SlotsOccupied,
                chargingStationEntity.Latitude,
                chargingStationEntity.Longitude);
        }

        public static List<ChargingStation> AssembleModelList(IList<DbChargingStation> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbChargingStation> AssembleEntityList(bool setIdsNull, IList<ChargingStation> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            else
                return models.Select(e => AssembleEntity(e.ChargingStationId, e)).ToList();
        }
    }
}
