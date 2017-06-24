using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using Microsoft.AspNetCore.Mvc;

namespace ecruise.Api.Controllers
{
    public class ConfigurationController : BaseController
    {
        public ConfigurationController(EcruiseContext context) : base(context)
        {
        }

        // GET: /Configuration
        [HttpGet(Name = "GetConfiguration")]
        public async Task<IActionResult> GetAsync()
        {
            // Forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Get the configuration from the database
            var configuration = await Context.Configurations.FindAsync((ulong)1);

            if (configuration == null)
                // Return that the configuration was not found
                return NotFound();

            // Return the configuration
            return Ok(configuration);
        }

        // PATCH: /Configuration/allowNewBookings
        [HttpPatch("allowNewBookings")]
        public async Task<IActionResult> PatchAsync([FromBody] bool allowNewBookings)
        {
            // Forbid if not admin
            if (!HasAccess())
                return Unauthorized();

            // Get current configuration from database
            var configuration = await Context.Configurations.FindAsync((ulong)1);

            // Set the attribute to the given value
            configuration.AllowNewBookings = allowNewBookings;

            // Save the changes
            await Context.SaveChangesAsync();

            // Return a reference to the patch object
            return Ok(new PostReference(configuration.ConfigurationId, $"{BasePath}/configuration/{configuration.ConfigurationId}"));
        }
    }
}