using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecruise.Api.Controllers
{
    public class StatisticsController : BaseController
    {
        public StatisticsController(EcruiseContext context) : base(context)
        {
        }

        // GET: api/Statistics
        [HttpGet(Name = "GetAllStatistics")]
        public async Task<IActionResult> Get()
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            var statistics = await Context.Statistics.ToListAsync();

            return Ok(StatisticAssembler.AssembleModelList(statistics));
        }

        // GET: api/Statistics/5
        [HttpGet("{date}", Name = "GetStatisticByDate")]
        public async Task<IActionResult> Get(string date)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Transform string to date
            DateTime requestedDateTime;
            if (DateTime.TryParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out requestedDateTime))
            {
                var statistic = await Context.Statistics.FindAsync(requestedDateTime.Date);

                if(statistic == null)
                    return NotFound(new Error(201, "Statistic of requested date does not exist.",
                        "An error occured. Please check the message for further information."));

                return Ok(StatisticAssembler.AssembleModel(statistic));
            }

            return BadRequest(new Error(1,
                "The date given was not formatted correctly. Date must be in following format: 'yyyy-MM-ddTHH:mm:ss.zzzZ'",
                "An error occured. Please check the message for further information."));
        }
    }
}
