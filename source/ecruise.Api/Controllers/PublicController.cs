using Microsoft.AspNetCore.Mvc;
using ecruise.Models;

namespace ecruise.Api.Controllers
{
    public class PublicController : BaseController
    {
        // POST: /Public/Login/login@ecruise.me
        [HttpPost("Login/{email}", Name = "Login")]
        public IActionResult Login([FromRoute] string email, [FromBody] string password)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            if (email == "login@ecruise.me" && password == "test123")
                return Ok(new { Id = 1, Token = "3AB904DECE2AA46ED4189AC0D68409DEB1B1DC0DD7EFD602F23D060A87E0D86C"});
            else
                return Unauthorized();
        }

        // GET: /Activate/login@ecruise.me/F3E64113EAC3432FFE968942674E98C6B01987F69EADEA90EEA1F8C809AB3DEE
        [HttpGet("Activate/{email}/{token}", Name = "ActivateAccount")]
        public IActionResult Activate([FromRoute] string email, [FromRoute] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            if (email == "login@ecruise.me" &&
                token == "F3E64113EAC3432FFE968942674E98C6B01987F69EADEA90EEA1F8C809AB3DEE")
                return NoContent();
            else
                return NotFound();
        }
    }
}
