using System;
using System.Collections.Generic;
using System.Linq;

using Car = ecruise.Models.Car;
using DbCar = ecruise.Database.Models.Car;

namespace ecruise.Models.Assemblers
{
    public static class CarAssembler
    {
        // ENum helper functions for BookingState
        public static string EnumToStringBookingState(Car.BookingStateEnum t)
        {
            switch (t)
            {
                case Car.BookingStateEnum.Available:
                    return "AVAILABLE";
                case Car.BookingStateEnum.Booked:
                    return "BOOKED";
                case Car.BookingStateEnum.Blocked:
                    return "BLOCKED";
                default:
                    throw new NotImplementedException();
            }
        }
        public static Car.BookingStateEnum StringToEnumBookingState(string e)
        {
            switch (e)
            {
                case "AVAILABLE":
                    return Car.BookingStateEnum.Available;
                case "BOOKED":
                    return Car.BookingStateEnum.Booked;
                case "BLOCKED":
                    return Car.BookingStateEnum.Blocked;
                default:
                    throw new NotImplementedException();
            }
        }

        // ENum helper functions for ChargingState
        public static string EnumToStringChargingState(Car.ChargingStateEnum t)
        {
            switch (t)
            {
                case Car.ChargingStateEnum.Charging:
                    return "CHARGING";
                case Car.ChargingStateEnum.Full:
                    return "FULL";
                case Car.ChargingStateEnum.Discharging:
                    return "DISCHARGING";
                default:
                    throw new NotImplementedException();
            }

        }
        public static Car.ChargingStateEnum StringToEnumChargingState(string e)
        {
            switch (e)
            {
                case "CHARGING":
                    return Car.ChargingStateEnum.Charging;
                case "FULL":
                    return Car.ChargingStateEnum.Full;
                case "DISCHARGING":
                    return Car.ChargingStateEnum.Discharging;
                default:
                    throw new NotImplementedException();
            }
        }



        public static DbCar AssembleEntity(ulong carId, Car carModel)
        {
            DbCar carEntity = new DbCar
            {
                CarId = carId != 0 ? carId : carModel.CarId,
                LicensePlate = carModel.LicensePlate,
                ChargingState = EnumToStringChargingState(carModel.ChargingState),
                BookingState = EnumToStringBookingState(carModel.BookingState),
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
                StringToEnumChargingState(carEntity.ChargingState),
                StringToEnumBookingState(carEntity.BookingState),
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
