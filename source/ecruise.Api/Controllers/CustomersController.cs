using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ecruise.Models;
using ecruise.Models.Assemblers;
using DbCustomer = ecruise.Database.Models.Customer;

namespace ecruise.Api.Controllers
{
    public class CustomersController : BaseController
    {
        // TODO: Requires to be admin
        // GET: /Customers
        [HttpGet(Name = "GetAllCustomers")]
        public IActionResult GetAll()
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            List<DbCustomer> customers = Context.Customers.ToList();
            if (customers.Count == 0)
                return NoContent();

            return Ok(CustomerAssembler.AssembleModelList(customers));
        }

        // TODO: Customer can only request it's own data \
        //         Admins can view all customers.
        // GET: /Customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult GetOne(ulong id)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            return Ok(CustomerAssembler.AssembleModel(customer));
        }

        // PATCH: /Customers/5/password
        [HttpPatch("{id}/password", Name = "UpdateCustomerPassword")]
        public IActionResult UpdatePassword(ulong id, [FromBody] string password)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

            // TODO(Lyrex): Implement proper update password method

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            string passwordSalt = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 16);

            SHA256 alg = SHA256.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(password + passwordSalt));
            string passwordHash = BitConverter.ToString(result).ToLowerInvariant().Replace("-", "");

            customer.PasswordHash = passwordHash;
            customer.PasswordSalt = passwordSalt;

            // invalidate all old login tokens
            var customerTokens =
                Context.CustomerTokens
                    .Where(t => t.Type == "LOGIN")
                    .Where(t => t.ExpireDate == null || t.ExpireDate >= DateTime.UtcNow)
                    .Where(t => t.CustomerId == customer.CustomerId)
                    .ToList();

            foreach (var t in customerTokens)
                t.ExpireDate = DateTime.UtcNow;

            Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/email
        [HttpPatch("{id}/email", Name = "UpdateCustomerEmail")]
        public IActionResult UpdateEmail(ulong id, [FromBody] string email)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

            // TODO(Lyrex): Implement proper update email method

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            customer.Email = email;

            // invalidate all old login tokens
            var tokens =
                Context.CustomerTokens
                    .Where(t => t.Type == "LOGIN")
                    .Where(t => t.ExpireDate == null || t.ExpireDate >= DateTime.UtcNow)
                    .Where(t => t.CustomerId == customer.CustomerId)
                    .ToList();

            foreach (var t in tokens)
                t.ExpireDate = DateTime.UtcNow;

            Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/phone-number
        [HttpPatch("{id}/phone-number", Name = "UpdateCustomerPhoneNumber")]
        public IActionResult UpdatePhoneNumber(ulong id, [FromBody] string phoneNumber)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            customer.PhoneNumber = phoneNumber;
            Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/address
        [HttpPatch("{id}/address", Name = "UpdateCustomerAddress")]
        public IActionResult UpdateAddress(ulong id, [FromBody] Address address)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            using (var transaction = Context.Database.BeginTransaction())
            {
                customer.Country = address.Country;
                customer.City = address.City;
                customer.ZipCode = address.ZipCode;
                customer.Street = address.Street;
                customer.HouseNumber = address.HouseNumber;
                customer.AddressExtraLine = address.AdressExtraLine;

                transaction.Commit();
                Context.SaveChangesAsync();
            }
            return Ok(new PostReference(1, $"{BasePath}/customer/{customer.CustomerId}"));
        }

        // PATCH: /Customers/5/verified
        [HttpPatch("{id}/verified", Name = "UpdateCustomerVerified")]
        public IActionResult UpdateVerified(ulong id, [FromBody] bool verified)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);

            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            customer.Verified = verified;
            Context.SaveChangesAsync();

            return Ok(new PostReference(1, $"{BasePath}/customer/{customer.CustomerId}"));
        }

        // PATCH: /Customers/5/chipcarduid
        [HttpPatch("{id}/chipcarduid", Name = "UpdateCustomerChipCardUid")]
        public IActionResult UpdateChipCardUid(ulong id, [FromBody] string chipCardUid)
        {
            // forbid if user is accessing different user's ressources
            if (!HasAccess(id))
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            customer.ChipCardUid = chipCardUid;
            Context.SaveChangesAsync();

            return Ok(new PostReference(1, $"{BasePath}/customer/{customer.CustomerId}"));
        }

        // GET: /Customers/by-lastname/5
        [HttpGet("by-lastname/{name}", Name = "GetCustomerByLastName")]
        public IActionResult GetCustomerByLastName(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            ImmutableList<DbCustomer> customers = Context.Customers
                // only query customers, the current customer has access to
                .Where(c => c.CustomerId == AuthentificatedCustomerId || AuthentificatedCustomerId == 1)
                // query customers by last name
                .Where(c => c.LastName == name)
                .ToImmutableList();

            if (customers.Count == 0)
                return NoContent();

            return Ok(CustomerAssembler.AssembleModelList(customers));
        }
    }
}
