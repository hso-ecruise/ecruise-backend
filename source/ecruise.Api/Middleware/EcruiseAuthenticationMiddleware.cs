using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using EcruiseContext = ecruise.Database.Models.EcruiseContext;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;

namespace ecruise.Api.Middleware
{
    public class EcruiseAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly EcruiseContext _dbContext;

        public EcruiseAuthenticationMiddleware(RequestDelegate next, EcruiseContext dbContext)
        {
            _next = next;
            _dbContext = dbContext;
        }

        public async Task Invoke(HttpContext context)
        {
            // only apply to non-public routes
            if (!context.Request.Path.StartsWithSegments("/v1/public", StringComparison.OrdinalIgnoreCase))
            {
                string authToken = context.Request.Headers["access_token"];

                // auth header not set
                if (authToken == null)
                {
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }

                _dbContext.Database.EnsureCreated();

                DbCustomerToken customerToken =
                    _dbContext.CustomerTokens
                        .Where(t => t.ExpireDate == null || t.ExpireDate > DateTime.UtcNow)
                        .Where(t => t.Type == "LOGIN")
                        .FirstOrDefault(t => t.Token == authToken);

                // no valid customer token found
                if (customerToken == null)
                {
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}
