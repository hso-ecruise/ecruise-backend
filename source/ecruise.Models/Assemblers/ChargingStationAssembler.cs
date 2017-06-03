using System.Collections.Generic;
using System.Linq;

using ChargingStation = ecruise.Models.ChargingStation;
using DbChargingStation = ecruise.Database.Models.ChargingStation;

namespace ecruise.Models.Assemblers
{
    public class ChargingStationAssembler
    {
        public static DbChargingStation AssembleEntity(ChargingStation chargingStationModel)
        {
            return new DbChargingStation
            {
                ChargingStationId = chargingStationModel.ChargingStationId,
                Slots = chargingStationModel.Slots,
                SlotsOccupied = chargingStationModel.SlotsOccupuied,
                Latitude = chargingStationModel.Latitude,
                Longitude = chargingStationModel.Longitude
            };
        }
        public static ChargingStation AssembleModel(DbChargingStation chargingStationEntity)

        {
            return new ChargingStation(
                chargingStationEntity.ChargingStationId,
                chargingStationEntity.Slots,
                chargingStationEntity.SlotsOccupied,
                chargingStationEntity.Latitude,
                chargingStationEntity.Longitude);
        }

        public static List<ChargingStation> AssembleModelList(IList<DbChargingStation> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbChargingStation> AssembleEntityList(IList<ChargingStation> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}
