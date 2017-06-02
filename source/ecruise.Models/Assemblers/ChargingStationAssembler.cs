using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    public class ChargingStationAssembler
    {
        public static Database.Models.ChargingStation AssembleEntity(ChargingStation chargingStationModel)
        {
            return new Database.Models.ChargingStation
            {
                ChargingStationId = chargingStationModel.ChargingStationId,
                Slots = chargingStationModel.Slots,
                SlotsOccupied = chargingStationModel.SlotsOccupuied,
                Latitude = chargingStationModel.Latitude,
                Longitude = chargingStationModel.Longitude
            };
        }

        public static ChargingStation AssembleModel(Database.Models.ChargingStation chargingStationEntity)
        {
            return new ChargingStation(
                (uint)chargingStationEntity.ChargingStationId,
                chargingStationEntity.Slots,
                chargingStationEntity.SlotsOccupied,
                chargingStationEntity.Latitude,
                chargingStationEntity.Longitude);
        }
    }
}
