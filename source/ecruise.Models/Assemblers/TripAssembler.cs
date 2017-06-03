using System.Collections.Generic;
using System.Linq;
using Trip = ecruise.Models.Trip;
using DbTrip = ecruise.Database.Models.Trip;

namespace ecruise.Models.Assemblers
{
    public class TripAssembler
    {
        public static DbTrip AssembleEntity(Trip tripModel)
        {
            return new DbTrip
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

        public static Trip AssembleModel(DbTrip tripEntity)
        {
            return new Trip(
                tripEntity.TripId,
                tripEntity.CarId,
                tripEntity.CustomerId,
                tripEntity.StartDate,
                tripEntity.EndDate,
                tripEntity.StartChargingStationId,
                tripEntity.EndChargingStationId,
                tripEntity.DistanceTravelled);
        }

        public static List<Trip> AssembleModelList(IList<DbTrip> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbTrip> AssembleEntityList(IList<Trip> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}
