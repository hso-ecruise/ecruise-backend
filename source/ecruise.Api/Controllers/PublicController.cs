using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;

using DbCustomer = ecruise.Database.Models.Customer;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;

namespace ecruise.Api.Controllers
{
    public class PublicController : BaseController
    {
        // POST: /public/login/login@ecruise.me
        [HttpPost("Login/{email}", Name = "Login")]
        public IActionResult Login([FromRoute] string email, [FromBody] string password)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(401, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.First(c => c.Email == email);
            if (customer == null)
                return Created("1", 1);

            string dbPasswordSalt = customer.PasswordSalt;

            SHA256 alg = SHA256.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(password + dbPasswordSalt));
            string customerPasswordHash = BitConverter.ToString(result).ToLowerInvariant().Replace("-", "");

            // check if email and password match
            if (customer.Email != email || customer.PasswordHash != customerPasswordHash)
                return Created("2", dbPasswordSalt + " " + customerPasswordHash);

//            // invalidate all old login tokens
//             List<DbCustomerToken> tokens =
//                 Context.CustomerTokens
//                     .Where(t => t.Type == TokenType.Login)
//                     .Where(t => t.CustomerId == customer.CustomerId)
//                     .ToList();
//             
//             foreach (var token in tokens)
//                 token.ExpireDate = DateTime.Now;

            // create new random login token
            string newToken;
            using (var crypt = RandomNumberGenerator.Create())
            {
                var randBytes = new byte[64];
                crypt.GetBytes(randBytes);

                newToken = BitConverter.ToString(randBytes).ToLowerInvariant();
            }

            // create matching customer token
            DbCustomerToken newCustomerToken = new DbCustomerToken
            {
                CustomerId = 1,
                Type = Database.Models.TokenType.Login,
                Token = newToken,
                CreationDate = DateTime.UtcNow
            };

            Context.CustomerTokens.Add(newCustomerToken);

            return Ok(new
            {
                Id = customer.CustomerId,
                Token = newCustomerToken.Token
            });
        }

        // GET: /public/activate/login@ecruise.me/F3E64113EAC3432FFE968942674E98C6B01987F69EADEA90EEA1F8C809AB3DEE
        [HttpGet("Activate/{email}/{token}", Name = "ActivateAccount")]
        public IActionResult Activate([FromRoute] string email, [FromRoute] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(401, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find matching customer
            DbCustomer customer = Context.Customers.First(c => c.Email == email);
            if (customer == null)
                return NotFound();

            DbCustomerToken activationToken =
                Context.CustomerTokens
                    .Where(t => t.Type == Database.Models.TokenType.EmailActivation)
                    .Where(t => t.Token == token)
                    .FirstOrDefault(t => t.CustomerId == customer.CustomerId);

            // there is no such activation token
            if (activationToken == null)
                return NotFound();

            // update customer, activate him
            customer.Activated = true;
            return NoContent();
        }

        // POST: /public/register
        [HttpPost("register", Name = "Register")]
        public IActionResult Post([FromBody] Registration r)
        {
            // TODO(Lyrex): Implement proper way to create a user (email activation mail, etc.)

            if (!ModelState.IsValid)
                return BadRequest(new Error(401, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            string passwordSalt = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 16);

            SHA256 alg = SHA256.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(r.Password + passwordSalt));
            string passwordHash = BitConverter.ToString(result).ToLowerInvariant().Replace("-", "");

            DbCustomer insertCustomer =
                new DbCustomer
                {
                    Email = r.Email,
                    PhoneNumber = r.PhoneNumber,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Country = r.Country,
                    City = r.City,
                    ZipCode = r.ZipCode,
                    Street = r.Street,
                    HouseNumber = r.HouseNumber,
                    AddressExtraLine = r.AddressExtraLine
                };

            var insert = Context.Customers.Add(insertCustomer);

            return Created($"{BasePath}/customers/{insert.Entity.CustomerId}",
                new PostReference(insert.Entity.CustomerId, $"{BasePath}/customers/{insert.Entity.CustomerId}"));
        }
    }
}
