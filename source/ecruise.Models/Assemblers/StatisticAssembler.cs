using System;
using System.Collections.Generic;
using System.Linq;
using DbStatistic = ecruise.Database.Models.Statistic;

namespace ecruise.Models.Assemblers
{
    public class StatisticAssembler
    {
        public static DbStatistic AssembleEntity(DateTime? date, Statistic statisticModel)
        {
            DbStatistic statisticEntity =
                new DbStatistic
                {
                    Date = date ?? statisticModel.Date,
                    Bookings = statisticModel.Bookings,
                    AverageChargeLevel = statisticModel.AverageChargeLevel,
                    CarsInUse = statisticModel.CarsInUse,
                    CarsCharging = statisticModel.CarsCharging
                };

            return statisticEntity;
        }

        public static Statistic AssembleModel(DbStatistic statisticEntity)
        {
            return new Statistic(
                statisticEntity.Date,
                statisticEntity.Bookings,
                statisticEntity.AverageChargeLevel,
                statisticEntity.CarsInUse,
                statisticEntity.CarsCharging
            );
        }

        public static List<Statistic> AssembleModelList(IList<DbStatistic> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbStatistic> AssembleEntityList(bool setIdsNull, IList<Statistic> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(null, e)).ToList();

            return models.Select(e => AssembleEntity(e.Date, e)).ToList();
        }
    }
}
