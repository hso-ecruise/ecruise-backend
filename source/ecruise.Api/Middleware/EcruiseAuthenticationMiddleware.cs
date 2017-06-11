using System;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;

namespace ecruise.Api.Middleware
{
    public class EcruiseAuthenticationMiddleware
    {
        private readonly DbContextOptions _dbContextOptions;
        private readonly RequestDelegate _next;

        public EcruiseAuthenticationMiddleware(RequestDelegate next, EcruiseContext dbContext)
        {
            _next = next;

            var optionsBuilder = new DbContextOptionsBuilder<EcruiseContext>();
            optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            _dbContextOptions = optionsBuilder.Options;
        }

        public async Task Invoke(HttpContext context)
        {
            // only apply to non-public routes
            if (!context.Request.Path.StartsWithSegments("/v1/public", StringComparison.OrdinalIgnoreCase) &&
                !context.Request.Path.ToString().Contains("/confirm-email/"))
            {
                string authToken = context.Request.Headers["access_token"];

                // auth header not set
                if (authToken == null)
                {
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }

                bool hasValidToken;
                using (var dbContext = new EcruiseContext(_dbContextOptions))
                {
                    var tokens = await dbContext.CustomerTokens
                        .Where(t => t.ExpireDate == null || t.ExpireDate > DateTime.UtcNow)
                        .Where(t => t.Type == "LOGIN")
                        .Where(t => t.Token == authToken).ToListAsync();

                    // Check if any
                    hasValidToken = tokens.Any();

                    // Touch token(s) to be valid 20 more minutes when still in use
                    if (tokens.Any())
                    {
                        tokens.ForEach(t => t.ExpireDate = DateTime.UtcNow.AddMinutes(20));

                        await dbContext.SaveChangesAsync();
                    }

                }

                // no valid customer token found
                if (!hasValidToken)
                {
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}
