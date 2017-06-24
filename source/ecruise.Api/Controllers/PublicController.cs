using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorLight;
using CustomerToken = ecruise.Models.CustomerToken;
using DbCustomer = ecruise.Database.Models.Customer;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;
using DbInvoice = ecruise.Database.Models.Invoice;

namespace ecruise.Api.Controllers
{
    public class PublicController : BaseController
    {
        private readonly IRazorLightEngine _razorEngine;

        public PublicController(EcruiseContext context, IRazorLightEngine razorEngine) : base(context)
        {
            _razorEngine = razorEngine;
        }

        // POST: /public/login/login@ecruise.me
        [HttpPost("Login/{email}", Name = "Login")]
        public async Task<IActionResult> LoginAsync([FromRoute] string email, [FromBody] string password)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(401, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = await Context.Customers
                .Where(c => c.Activated)
                .FirstOrDefaultAsync(c => c.Email == email);

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
            // List<Db> tokens =
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
                    DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(60)));

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
        public async Task<IActionResult> ActivateAsync([FromRoute] string email, [FromRoute] string token)
        {
            // Find matching customer
            DbCustomer customer = await Context.Customers.FirstOrDefaultAsync(c => c.Email == email);
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
        public async Task<IActionResult> PostAsync([FromBody] Registration r)
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
            DbCustomer insertCustomer = CustomerAssembler.AssembleEntity(0, r);
            insertCustomer.PasswordSalt = passwordSalt;
            insertCustomer.PasswordHash = passwordHash;

            // save customer to database
            var insert = await Context.Customers.AddAsync(insertCustomer);
            await Context.SaveChangesAsync();

            // generate customer authentification token for email activation
            var crypt = RandomNumberGenerator.Create();
            var randBytes = new byte[64];
            crypt.GetBytes(randBytes);

            string activationToken = BitConverter.ToString(randBytes).ToLowerInvariant().Replace("-", "");
            crypt.Dispose();

            var newCustomer = CustomerTokenAssembler.AssembleEntity(0,
                new CustomerToken(0, (uint)insert.Entity.CustomerId, CustomerToken.TokenTypeEnum.EmailActivation,
                    activationToken, DateTime.UtcNow, null));

            await Context.CustomerTokens.AddAsync(newCustomer);

            var save = Context.SaveChangesAsync();

            await CustomerAssembler.AssembleModel(insert.Entity).SendMail("Deine Registrierung bei eCruise",
                _razorEngine.Parse("CustomerRegistered.cshtml", new
                {
                    r.FirstName,
                    r.Email,
                    ActivationToken = activationToken
                })
            );

            await save;

            // Create blank invoice for customer
            DbInvoice newInvoice = new DbInvoice
            {
                Paid = false,
                TotalAmount = 0.0,
                CustomerId = newCustomer.CustomerId
            };

            // Change the new invoice to the database
            await Context.Invoices.AddAsync(newInvoice);
            await Context.SaveChangesAsync();

            return Created($"{BasePath}/customers/{insert.Entity.CustomerId}",
                new PostReference((uint)insert.Entity.CustomerId, $"{BasePath}/customers/{insert.Entity.CustomerId}"));
        }
    }
}
