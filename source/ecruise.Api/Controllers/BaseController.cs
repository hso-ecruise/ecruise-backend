using System.Linq;
using ecruise.Database;
using ecruise.Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace ecruise.Api.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class BaseController : Controller
    {
        public readonly string BasePath = "v1";

        protected static readonly EcruiseContext Context = new EcruiseContextFactory().Create(options: null);

        protected string GetModelStateErrorString()
        {
            return string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }
    }
}