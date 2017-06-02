using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    public class CarChargingStationAssembler
    {
        public static Database.Models.CarChargingStation AssembleEntity(CarChargingStation carChargingStationModel)
        {
            return new Database.Models.CarChargingStation
            {
                CarChargingStationId = carChargingStationModel.CarChargingStationId,
                CarId = carChargingStationModel.CarId,
                ChargeEnd = carChargingStationModel.ChargeEnd,
                ChargeStart = carChargingStationModel.ChargeStart,
                ChargingStationId = carChargingStationModel.CarChargingStationId
            };
        }

        public static CarChargingStation AssembleModel(Database.Models.CarChargingStation carChargingStationEntity)
        {
            return new CarChargingStation(
                (uint)carChargingStationEntity.CarChargingStationId,
                (uint)carChargingStationEntity.CarId,
                (uint)carChargingStationEntity.ChargingStationId,
                carChargingStationEntity.ChargeStart,
                carChargingStationEntity.ChargeEnd
            );
        }
    }
}
