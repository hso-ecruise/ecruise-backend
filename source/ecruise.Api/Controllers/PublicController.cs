using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbCustomer = ecruise.Database.Models.Customer;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;

namespace ecruise.Api.Controllers
{
    [AllowAnonymous]
    public class PublicController : BaseController
    {
        // POST: /public/login/login@ecruise.me
        [HttpPost("Login/{email}", Name = "Login")]
        public async Task<IActionResult> Login([FromRoute] string email, [FromBody] string password)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(401, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = await Context.Customers
                .Where(c => c.Activated)
                .FirstAsync(c => c.Email == email);

            if (customer == null)
                return Unauthorized();

            string dbPasswordSalt = customer.PasswordSalt;

            SHA256 alg = SHA256.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(password + dbPasswordSalt));
            string customerPasswordHash = BitConverter.ToString(result).ToLowerInvariant().Replace("-", "");

            // check if email and password match
            if (customer.Email != email || customer.PasswordHash != customerPasswordHash)
                return Unauthorized();

            //// invalidate all old login tokens
            // List<DbCustomerToken> tokens =
            //     Context.CustomerTokens
            //         .Where(t => t.Type == TokenType.Login)
            //         .Where(t => t.CustomerId == customer.CustomerId)
            //         .ToList();

            // foreach (var token in tokens)
            //     token.ExpireDate = DateTime.UtcNow;

            // create new random login token
            string newToken;
            using (var crypt = RandomNumberGenerator.Create())
            {
                var randBytes = new byte[64];
                crypt.GetBytes(randBytes);

                newToken = BitConverter.ToString(randBytes).ToLowerInvariant().Replace("-", "");
            }

            // create matching customer token
            DbCustomerToken newCustomerToken = CustomerTokenAssembler.AssembleEntity(0,
                new CustomerToken(0, (uint)customer.CustomerId, CustomerToken.TokenTypeEnum.Login, newToken,
                    DateTime.UtcNow, null));

            await Context.CustomerTokens.AddAsync(newCustomerToken);
            await Context.SaveChangesAsync();

            return Ok(new
            {
                Id = customer.CustomerId,
                newCustomerToken.Token
            });
        }

        // GET: /public/activate/login@ecruise.me/F3E64113EAC3432FFE968942674E98C6B01987F69EADEA90EEA1F8C809AB3DEE
        [HttpGet("activate/{email}/{token}", Name = "ActivateAccount")]
        public async Task<IActionResult> Activate([FromRoute] string email, [FromRoute] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(401, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find matching customer
            DbCustomer customer = await Context.Customers.FirstAsync(c => c.Email == email);
            if (customer == null)
                return Redirect("https://ecruise.me/nocustomer");

            DbCustomerToken activationToken =
                await Context.CustomerTokens
                    .Where(t => t.Type == "EMAIL_ACTIVATION")
                    .Where(t => t.ExpireDate == null || t.ExpireDate >= DateTime.UtcNow)
                    .Where(t => string.Equals(t.Token, token, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefaultAsync(t => t.CustomerId == customer.CustomerId);

            // there is no such activation token
            if (activationToken == null)
                return Redirect("https://ecruise.me/notoken");

            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                // update customer, activate him
                customer.Activated = true;

                // invalidate activation token
                activationToken.ExpireDate = DateTime.UtcNow;

                transaction.Commit();

                await Context.SaveChangesAsync();
            }

            return Redirect("https://ecruise.me/start");
        }

        // POST: /public/register
        [HttpPost("register", Name = "Register")]
        public async Task<IActionResult> Post([FromBody] Registration r)
        {
            // TODO(Lyrex): Implement proper way to create a user (email activation mail, etc.)

            if (!ModelState.IsValid)
                return BadRequest(new Error(301, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // Check if username already in use
            if (await Context.Customers.AnyAsync(c => c.Email == r.Email))
                return BadRequest(new Error(401, "Email address already in use",
                    "The email you are trying to use to register is already in use."));

            // create customer password salt
            string passwordSalt = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 16);

            // create customer password hash
            SHA256 alg = SHA256.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(r.Password + passwordSalt));
            string passwordHash = BitConverter.ToString(result).ToLowerInvariant().Replace("-", "");

            // create database user model
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

            // save customer to database
            var insert = await Context.Customers.AddAsync(insertCustomer);
            await Context.SaveChangesAsync();

            // generate customer authentification token for email activation
            var crypt = RandomNumberGenerator.Create();
            var randBytes = new byte[64];
            crypt.GetBytes(randBytes);

            string activationToken = BitConverter.ToString(randBytes).ToLowerInvariant().Replace("-", "");
            crypt.Dispose();

            await Context.CustomerTokens.AddAsync(
                CustomerTokenAssembler.AssembleEntity(0,
                    new CustomerToken(0, (uint)insert.Entity.CustomerId, CustomerToken.TokenTypeEnum.EmailActivation,
                        activationToken, DateTime.UtcNow, null)
                )
            );
            var save = Context.SaveChangesAsync();

            await CustomerAssembler.AssembleModel(insert.Entity).SendMail("Deine Registrierung bei eCruise",
                string.Format(
                    @"Hallo {0},<br>
herzlich willkommen bei eCruise!<br>
<br>
Du bist noch nicht ganz fertig mit deiner Registrierung. Um sie abzuschlieﬂen, fehlt nur noch ein kleiner Schritt:<br>
Mit einem Klick auf den folgenden Link best‰tigst du deine Anmeldung:<br>
<br>
<a href=""https://api.ecruise.me/v1/public/activate/{1}/{2}"">https://api.ecruise.me/v1/public/activate/{1}/{2}</a><br>
<br>
Wir freuen uns auf dich!<br>
Dein eCruise-Team", r.FirstName, r.Email, activationToken));

            await save;
            return Created($"{BasePath}/customers/{insert.Entity.CustomerId}",
                new PostReference((uint)insert.Entity.CustomerId, $"{BasePath}/customers/{insert.Entity.CustomerId}"));
        }
    }
}
