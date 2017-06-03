using System.Collections.Generic;
using System.Linq;
using ecruise.Database.Models;

using Car = ecruise.Models.Car;
using DbCar = ecruise.Database.Models.Car;

namespace ecruise.Models.Assemblers
{
    public static class CarAssembler
    {
        public static DbCar AssembleEntity(ulong carId, Car carModel)
        {
            DbCar carEntity = new DbCar
            {
                CarId = carId != 0 ? carId : carModel.CarId,
                LicensePlate = carModel.LicensePlate,
                ChargingState = (ChargingState)carModel.ChargingState,
                BookingState = (BookingState) carModel.BookingState,
                Milage = carModel.Mileage,
                ChargeLevel = carModel.ChargeLevel,
                Kilowatts = carModel.Kilowatts,
                Manufacturer = carModel.Manufacturer,
                Model = carModel.Model,
                YearOfConstruction = carModel.YearOfConstruction,
                LastKnownPositionLatitude = carModel.LastKnownPositionLatitude,
                LastKnownPositionLongitude = carModel.LastKnownPositionLongitude,
                LastKnownPositionDate = carModel.LastKnownPositionDate
            };

            return carEntity;
        }

        public static Car AssembleModel(DbCar carEntity)
        {
            return new Car(
                (uint)carEntity.CarId,
                carEntity.LicensePlate,
                (Car.ChargingStateEnum)carEntity.ChargingState,
                (Car.BookingStateEnum)carEntity.BookingState,
                carEntity.Milage,
                carEntity.ChargeLevel,
                carEntity.Kilowatts,
                carEntity.Manufacturer,
                carEntity.Model,
                carEntity.YearOfConstruction,
                carEntity.LastKnownPositionLatitude,
                carEntity.LastKnownPositionLongitude,
                carEntity.LastKnownPositionDate
            );
        }

        public static List<Car> AssembleModelList(IList<DbCar> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbCar> AssembleEntityList(bool setIdsNull, IList<Car> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            else
                return models.Select(e => AssembleEntity(e.CarId, e)).ToList();
        }
    }
}
