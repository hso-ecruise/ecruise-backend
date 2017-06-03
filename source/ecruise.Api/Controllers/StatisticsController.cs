using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ecruise.Models;
using System.Globalization;
using Statistic = ecruise.Models.Statistic;

namespace ecruise.Api.Controllers
{
    [Route("v1/Statistics")]
    public class StatisticsController : BaseController
    {
        // GET: api/Statistics
        [HttpGet(Name = "GetAllStatistics")]
        public IActionResult Get()
        {
            try
            {
                List<Statistic> list = new List<Statistic>()
                {
                    new Statistic(new DateTime(2017, 05, 30, 0, 0, 0), 28, 93, 15, 30),
                    new Statistic(new DateTime(2017, 05, 31, 0, 0, 0), 30, 89, 14, 31)
                };

                return Ok(list);
            }
            catch (Exception e)
            {
                return BadRequest(new Error(1, e.Message,
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: api/Statistics/5
        [HttpGet("{date}", Name = "GetStatisticByDate")]
        public IActionResult Get(string date)
        {
            // Transform string to date
            DateTime requestedDateTime;
            if (DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out requestedDateTime))
            {
                return Ok(new Statistic(new DateTime(2017, 05, 30, 0, 0, 0), 42, 90, 16, 31));
            }
            else
            {
                return BadRequest(new Error(1, "The date given was not formatted correctly. Date must be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'",
                    "An error occured. Please check the message for further information."));
            }
        }
    }
}
