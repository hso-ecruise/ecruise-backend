using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    public class TripAssembler
    {
        public static Database.Models.Trip AssembleEntity(Trip tripModel)
        {
            return new Database.Models.Trip
            {
                TripId = tripModel.TripId,
                CarId = tripModel.CarId,
                CustomerId = tripModel.CustomerId,
                StartDate = tripModel.StartDate,
                EndDate = tripModel.EndDate,
                StartChargingStationId = tripModel.StartChargingStationId,
                EndChargingStationId = tripModel.EndChargingStationId,
                DistanceTravelled = tripModel.DistanceTravelled
            };
        }

        public static Trip AssembleModel(Database.Models.Trip tripEntity)
        {
            return new Trip(
                (uint)tripEntity.TripId,
                (uint?)tripEntity.CarId,
                (uint)tripEntity.CustomerId,
                tripEntity.StartDate,
                tripEntity.EndDate,
                tripEntity.StartChargingStationId,
                tripEntity.EndChargingStationId,
                tripEntity.DistanceTravelled);
        }
    }
}
