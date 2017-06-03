using System;
using System.Linq;
using ecruise.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ecruise.Api.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class BaseController : Controller
    {
        public readonly string BasePath = "v1";

        protected static readonly EcruiseContext Context = Api.Startup.Context;

        protected string GetModelStateErrorString()
        {
            return string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }

        protected uint GetCustomerIdByToken(string token)
        {
            CustomerToken customerToken =
                Context.CustomerTokens.FirstOrDefault(t => (t.Token == token && t.ExpireDate > DateTime.UtcNow));
            return (uint?)customerToken?.CustomerId ?? 0;
        }
    }
}
