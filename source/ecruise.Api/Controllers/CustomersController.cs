using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorLight;
using Customer = ecruise.Models.Customer;
using DbCustomer = ecruise.Database.Models.Customer;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;

namespace ecruise.Api.Controllers
{
    public class CustomersController : BaseController
    {
        private readonly IRazorLightEngine _razorEngine;

        public CustomersController(EcruiseContext context, IRazorLightEngine razorEngine) : base(context)
        {
            _razorEngine = razorEngine;
        }

        // GET: /Customers
        [HttpGet(Name = "GetAllCustomers")]
        public async Task<IActionResult> GetAllAsync()
        {
            // create a list of all customers
            List<DbCustomer> customers = await Context.Customers
                // query only customers the current customer has access to
                .Where(c => HasAccess(c.CustomerId))
                .ToListAsync();

            // return 203 No Content if there are no customers
            if (customers.Count == 0)
                return NoContent();

            return Ok(CustomerAssembler.AssembleModelList(customers));
        }

        // GET: /Customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> GetOneAsync(ulong id)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested customer
            DbCustomer customer = await Context.Customers.FindAsync(id);

            // return error if customer was not found
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            return Ok(CustomerAssembler.AssembleModel(customer));
        }

        // PATCH: /Customers/5/password
        [HttpPatch("{id}/password", Name = "UpdateCustomerPassword")]
        public async Task<IActionResult> UpdatePasswordAsync(ulong id, [FromBody] string password)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Unauthorized();

            // validate user input
            if (string.IsNullOrEmpty(password) || !ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested customer
            DbCustomer customer = Context.Customers.Find(id);

            // return error if customer was not found
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            // generate password salt
            string passwordSalt = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 16);

            // calculate password hash
            SHA256 alg = SHA256.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(password + passwordSalt));
            string passwordHash = BitConverter.ToString(result).ToLowerInvariant().Replace("-", "");

            // update customer using a transaction
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                // update customer hash
                customer.PasswordHash = passwordHash;
                customer.PasswordSalt = passwordSalt;

                // invalidate all old login tokens
                await Context.CustomerTokens
                    .Where(t => t.Type == "LOGIN")
                    .Where(t => t.ExpireDate == null || t.ExpireDate >= DateTime.UtcNow)
                    .Where(t => t.CustomerId == customer.CustomerId)
                    .ForEachAsync(t => t.ExpireDate = DateTime.UtcNow);

                transaction.Commit();

                await Context.SaveChangesAsync();
            }

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/email
        [HttpPatch("{id}/email", Name = "UpdateCustomerEmail")]
        public async Task<IActionResult> UpdateEmailAsync(ulong id, [FromBody] string email)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Unauthorized();

            // validate user input
            if (string.IsNullOrEmpty(email) || !ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested customer
            DbCustomer customer = await Context.Customers.FindAsync(id);

            // return error if customer was not found
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            // craft customer model
            Customer c = CustomerAssembler.AssembleModel(customer);

            // generate new phase 1 confirmation token
            string newToken;
            using (var crypt = RandomNumberGenerator.Create())
            {
                var randBytes = new byte[64];
                crypt.GetBytes(randBytes);

                newToken = BitConverter.ToString(randBytes).ToLowerInvariant().Replace("-", "");
            }

            // save new token to database
            DbCustomerToken dbt = new DbCustomerToken
            {
                CustomerId = c.CustomerId,
                Type = "EMAIL_CHANGE_PHASE_1",
                Token = newToken,
                CreationDate = DateTime.UtcNow
            };

            await Context.CustomerTokens.AddAsync(dbt);
            await Context.SaveChangesAsync();

            // send confirmation email
            await c.SendMail("Deine neue E-Mail-Adresse",
                _razorEngine.Parse("CustomerConfirmEmailChange.cshtml", new
                {
                    customerId = c.CustomerId,
                    firstName = c.FirstName,
                    newMail = email,
                    confirmationToken = newToken
                })
            );

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/phone-number
        [HttpPatch("{id}/phone-number", Name = "UpdateCustomerPhoneNumber")]
        public async Task<IActionResult> UpdatePhoneNumberAsync(ulong id, [FromBody] string phoneNumber)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Unauthorized();

            // validate user input
            if (string.IsNullOrEmpty(phoneNumber) || !ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested customer
            DbCustomer dbCustomer = await Context.Customers.FindAsync(id);

            // return error if customer was not found
            if (dbCustomer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            // update customer phone number
            dbCustomer.PhoneNumber = phoneNumber;

            await Context.SaveChangesAsync();

            // send confirmation email
            var customer = CustomerAssembler.AssembleModel(dbCustomer);

            await customer.SendMail("Anpassung deiner Daten",
                _razorEngine.Parse("CustomerDataChanged.cshtml", new
                {
                    customer.FirstName,
                    customer.LastName,
                    customer.PhoneNumber,
                    customer.Street,
                    customer.HouseNumber,
                    customer.AddressExtraLine,
                    customer.ZipCode,
                    customer.City,
                    customer.Country
                }));

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/address
        [HttpPatch("{id}/address", Name = "UpdateCustomerAddress")]
        public async Task<IActionResult> UpdateAddressAsync(ulong id, [FromBody] Address address)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Unauthorized();

            // validate user input
            if (address == null || !ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested customer
            DbCustomer dbCustomer = await Context.Customers.FindAsync(id);

            // return error if customer was not found
            if (dbCustomer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            // update the customer address data
            using (var transaction = Context.Database.BeginTransaction())
            {
                dbCustomer.Country = address.Country;
                dbCustomer.City = address.City;
                dbCustomer.ZipCode = address.ZipCode;
                dbCustomer.Street = address.Street;
                dbCustomer.HouseNumber = address.HouseNumber;
                dbCustomer.AddressExtraLine = address.AddressExtraLine;

                transaction.Commit();

                await Context.SaveChangesAsync();
            }


            // send confirmation email
            var customer = CustomerAssembler.AssembleModel(dbCustomer);

            await customer.SendMail("�nderung deiner Daten",
                _razorEngine.Parse("CustomerDataChanged.cshtml", new
                {
                    customer.FirstName,
                    customer.LastName,
                    customer.PhoneNumber,
                    customer.Street,
                    customer.HouseNumber,
                    customer.AddressExtraLine,
                    customer.ZipCode,
                    customer.City,
                    customer.Country
                }));

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/verified
        [HttpPatch("{id}/verified", Name = "UpdateCustomerVerified")]
        public async Task<IActionResult> UpdateVerifiedAsync(ulong id, [FromBody] bool verified)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess())
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested customer
            DbCustomer customer = await Context.Customers.FindAsync(id);

            // return error if customer was not found
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            // update customer verified status
            customer.Verified = verified;

            await Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/chipcarduid
        [HttpPatch("{id}/chipcarduid", Name = "UpdateCustomerChipCardUid")]
        public async Task<IActionResult> UpdateChipCardUidAsync(ulong id, [FromBody] string chipCardUid)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess())
                return Unauthorized();

            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find the requested customer
            DbCustomer customer = await Context.Customers.FindAsync(id);

            // return error if customer was not found
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            // update customer chip card uid
            customer.ChipCardUid = chipCardUid;

            await Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // GET: /Customers/by-lastname/5
        [HttpGet("by-lastname/{name}", Name = "GetCustomerByLastName")]
        public async Task<IActionResult> GetCustomerByLastNameAsync(string name)
        {
            List<DbCustomer> customers = await Context.Customers
                // only query customers, the current customer has access to
                .Where(c => HasAccess(c.CustomerId))
                // query customers by last name
                .Where(c => c.LastName == name)
                .ToListAsync();

            // return 203 No Content if there are no matching customers
            if (customers.Count == 0)
                return NoContent();

            return Ok(CustomerAssembler.AssembleModelList(customers));
        }

        // GET /customer/{customerId}/confirm-email/{newMail}/{token}
        [HttpGet("{id}/confirm-email/{newMail}/{token}", Name = "CustomerConfirmEmailChange")]
        public async Task<IActionResult> ConfirmEmailChangeAsync(ulong id, string newMail, string token)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // find requested token in database
            DbCustomerToken dbToken = await Context.CustomerTokens
                .Where(t => t.ExpireDate == null || t.ExpireDate >= DateTime.UtcNow)
                .FirstOrDefaultAsync(ct => string.Equals(ct.Token, token, StringComparison.OrdinalIgnoreCase));

            // the requested db token does not exist
            if (dbToken == null)
                return NotFound(new Error(202, "Token does not exist.",
                    $"There is such token: {token}."));

            // the associated token does not belong to that customer
            if (dbToken.CustomerId != id)
                return StatusCode(StatusCodes.Status409Conflict);

            // find the requested customer
            DbCustomer dbCustomer = await Context.Customers.FindAsync(id);

            if (dbToken.Type == "EMAIL_CHANGE_PHASE_1")
            {
                // craft customer model
                Customer c = CustomerAssembler.AssembleModel(dbCustomer);

                // generate new phase 2confirmation token
                string newToken;
                using (var crypt = RandomNumberGenerator.Create())
                {
                    var randBytes = new byte[64];
                    crypt.GetBytes(randBytes);

                    newToken = BitConverter.ToString(randBytes).ToLowerInvariant().Replace("-", "");
                }

                // save new token to database
                DbCustomerToken dbt = new DbCustomerToken
                {
                    CustomerId = c.CustomerId,
                    Type = "EMAIL_CHANGE_PHASE_2",
                    Token = newToken,
                    CreationDate = DateTime.UtcNow
                };

                await Context.CustomerTokens.AddAsync(dbt);
                await Context.SaveChangesAsync();

                // Send second confirmation mail 
                await c.SendMail("Best�tige deine eMail-�nderung",
                    _razorEngine.Parse("CustomerConfirmEmailChange.cshtml", new
                    {
                        customerId = c.CustomerId,
                        firstName = c.FirstName,
                        newMail,
                        confirmationToken = newToken
                    })
                );
            }
            else if (dbToken.Type == "EMAIL_CHANGE_PHASE_2")
            {
                // Change eMail
                dbCustomer.Email = newMail;

                // craft customer model
                Customer c = CustomerAssembler.AssembleModel(dbCustomer);

                // invalidate all old login tokens
                await Context.CustomerTokens
                    .Where(t => t.Type == "LOGIN")
                    .Where(t => t.ExpireDate == null || t.ExpireDate >= DateTime.UtcNow)
                    .Where(t => t.CustomerId == c.CustomerId)
                    .ForEachAsync(t => t.ExpireDate = DateTime.UtcNow);

                // Send data change email
                await c.SendMail("Deine eMail-�nderung war erfolgreich",
                    _razorEngine.Parse("CustomerDataChanged.cshtml", c)
                );
            }

            // Expire Token
            dbToken.ExpireDate = DateTime.UtcNow;
            await Context.SaveChangesAsync();

            return NoContent();
        }
    }
}
