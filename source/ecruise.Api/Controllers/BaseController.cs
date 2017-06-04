using System;
using System.Linq;
using ecruise.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ecruise.Api.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class BaseController 
        : Controller
    {
        protected const string BasePath = "v1";

        protected readonly EcruiseContext Context = Startup.Context;

        // TODO: Check if it's persistent over requests or per-request instanciated
        protected ulong AuthentificatedCustomerId { get; set; }

        protected string GetModelStateErrorString()
        {
            return string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AuthentificatedCustomerId = GetCustomerIdByToken(Request.Headers["access_token"]);

            base.OnActionExecuting(context);
        }

        protected bool HasAccess(ulong customerId = 0)
        {
            // user is not authentificated
            if (AuthentificatedCustomerId == 0)
                return false;

            // customerId 1 is admin account
            if (AuthentificatedCustomerId == 1)
                return true;

            return (AuthentificatedCustomerId == customerId);
        }

        //public override void OnActionExecuted(ActionExecutedContext context)
        //{
        //    if (Context.ChangeTracker.HasChanges())
        //        Context.SaveChanges();

        //    base.OnActionExecuted(context);
        //}

        private ulong GetCustomerIdByToken(string token)
        {
            CustomerToken customerToken = Context.CustomerTokens
                .Where(t => t.ExpireDate == null || t.ExpireDate > DateTime.UtcNow)
                .Where(t => t.Type == "LOGIN")
                .FirstOrDefault(t => t.Token == token);

            return customerToken?.CustomerId ?? 0;
        } 
    }
}
