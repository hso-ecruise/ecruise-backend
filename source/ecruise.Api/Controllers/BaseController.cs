using System.Linq;
using ecruise.Database;
using ecruise.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Infrastructure;

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

        // Code to be executed after each action
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Check if changes were made
            if (Context.ChangeTracker.HasChanges())
                Context.SaveChanges();

            base.OnActionExecuted(context);
        }
    }
}