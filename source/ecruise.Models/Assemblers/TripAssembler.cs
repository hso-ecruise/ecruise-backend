using System.Collections.Generic;
using System.Linq;
using Trip = ecruise.Models.Trip;
using DbTrip = ecruise.Database.Models.Trip;

namespace ecruise.Models.Assemblers
{
    public class TripAssembler
    {
        public static DbTrip AssembleEntity(ulong tripId, Trip tripModel)
        {
            return new DbTrip
            {
                TripId = tripId != 0 ? tripId : tripModel.TripId,
                CarId = tripModel.CarId,
                CustomerId = tripModel.CustomerId,
                StartDate = tripModel.StartDate,
                EndDate = tripModel.EndDate,
                StartChargingStationId = tripModel.StartChargingStationId,
                EndChargingStationId = tripModel.EndChargingStationId,
                DistanceTravelled = tripModel.DistanceTravelled
            };
        }

        public static Trip AssembleModel(DbTrip tripEntity)
        {
            return new Trip(
                (uint)tripEntity.TripId,
                (uint?)tripEntity.CarId,
                (uint)tripEntity.CustomerId,
                tripEntity.StartDate,
                tripEntity.EndDate,
                (uint?)tripEntity.StartChargingStationId,
                (uint?)tripEntity.EndChargingStationId,
                tripEntity.DistanceTravelled);
        }

        public static List<Trip> AssembleModelList(IList<DbTrip> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbTrip> AssembleEntityList(bool setIdsNull, IList<Trip> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            else
                return models.Select(e => AssembleEntity(e.TripId, e)).ToList();
        }
    }
}
