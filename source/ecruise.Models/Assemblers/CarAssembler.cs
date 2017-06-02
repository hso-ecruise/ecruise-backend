using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ecruise.Database.Models;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace ecruise.Models.Assemblers
{
    public class CarAssembler
    {
        public static Database.Models.Car AssembleEntity(Car carModel)
        {
            Database.Models.Car carEntity = new Database.Models.Car
            {
                CarId = carModel.CarId,
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

        public static Car AssembleModel(Database.Models.Car carEntity)
        {
            return new Car(carEntity.CarId,
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
    }
}
