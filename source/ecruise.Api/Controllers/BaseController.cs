using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ecruise.Api.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class BaseController : Controller
    {
        public readonly string BasePath = "v1";

        protected string GetModelStateErrorString()
        {
            return string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }
    }
}