using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbCustomer = ecruise.Database.Models.Customer;

namespace ecruise.Api.Controllers
{
    public class CustomersController : BaseController
    {
        public CustomersController(EcruiseContext context) : base(context)
        {
        }

        // GET: /Customers
        [HttpGet(Name = "GetAllCustomers")]
        public async Task<IActionResult> GetAll()
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
        public async Task<IActionResult> GetOne(ulong id)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

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
        public async Task<IActionResult> UpdatePassword(ulong id, [FromBody] string password)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

            // validate user input
            if (!ModelState.IsValid)
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
        public async Task<IActionResult> UpdateEmail(ulong id, [FromBody] string email)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

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

            // update customer email
            customer.Email = email;

            // invalidate all old login tokens
            await Context.CustomerTokens
                .Where(t => t.Type == "LOGIN")
                .Where(t => t.ExpireDate == null || t.ExpireDate >= DateTime.UtcNow)
                .Where(t => t.CustomerId == customer.CustomerId)
                .ForEachAsync(t => t.ExpireDate = DateTime.UtcNow);

            await Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/phone-number
        [HttpPatch("{id}/phone-number", Name = "UpdateCustomerPhoneNumber")]
        public async Task<IActionResult> UpdatePhoneNumber(ulong id, [FromBody] string phoneNumber)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

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

            // update customer phone number
            customer.PhoneNumber = phoneNumber;

            await Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/address
        [HttpPatch("{id}/address", Name = "UpdateCustomerAddress")]
        public async Task<IActionResult> UpdateAddress(ulong id, [FromBody] Address address)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

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

            // update the customer address data
            using (var transaction = Context.Database.BeginTransaction())
            {
                customer.Country = address.Country;
                customer.City = address.City;
                customer.ZipCode = address.ZipCode;
                customer.Street = address.Street;
                customer.HouseNumber = address.HouseNumber;
                customer.AddressExtraLine = address.AdressExtraLine;

                transaction.Commit();

                await Context.SaveChangesAsync();
            }

            return Ok(new PostReference(1, $"{BasePath}/customer/{customer.CustomerId}"));
        }

        // PATCH: /Customers/5/verified
        [HttpPatch("{id}/verified", Name = "UpdateCustomerVerified")]
        public async Task<IActionResult> UpdateVerified(ulong id, [FromBody] bool verified)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

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

            return Ok(new PostReference(1, $"{BasePath}/customer/{customer.CustomerId}"));
        }

        // PATCH: /Customers/5/chipcarduid
        [HttpPatch("{id}/chipcarduid", Name = "UpdateCustomerChipCardUid")]
        public async Task<IActionResult> UpdateChipCardUid(ulong id, [FromBody] string chipCardUid)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return new ForbidResult();

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

            return Ok(new PostReference(1, $"{BasePath}/customer/{customer.CustomerId}"));
        }

        // GET: /Customers/by-lastname/5
        [HttpGet("by-lastname/{name}", Name = "GetCustomerByLastName")]
        public async Task<IActionResult> GetCustomerByLastName(string name)
        {
            // validate user input
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

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
    }
}
