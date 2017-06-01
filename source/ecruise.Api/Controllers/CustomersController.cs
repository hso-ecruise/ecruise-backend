using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
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
            List<DbCustomer> customers = Context.Customers.ToList();
            if (customers.Count == 0)
                return NoContent();

            return Ok(customers);
        }

        // TODO: Customer can only request it's own data \
        //         Admins can view all customers.
        // GET: /Customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult GetOne(uint id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            return Ok(customer);
        }

        // PATCH: /Customers/5/password
        [HttpPatch("{id}/password", Name = "UpdateCustomerPassword")]
        public IActionResult UpdatePassword(uint id, [FromBody] string password)
        {
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

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/email
        [HttpPatch("{id}/email", Name = "UpdateCustomerEmail")]
        public IActionResult UpdateEmail(uint id, [FromBody] string email)
        {
            // TODO(Lyrex): Implement proper update email method

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            customer.Email = email;

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/phone-number
        [HttpPatch("{id}/phone-number", Name = "UpdateCustomerPhoneNumber")]
        public IActionResult UpdatePhoneNumber(uint id, [FromBody] string phoneNumber)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            customer.PhoneNumber = phoneNumber;

            return Ok(new PostReference(id, $"{BasePath}/customer/{id}"));
        }

        // PATCH: /Customers/5/address
        [HttpPatch("{id}/address", Name = "UpdateCustomerAddress")]
        public IActionResult UpdateAddress(uint id, [FromBody] Address address)
        {
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
            }
            return Ok(new PostReference(1, $"{BasePath}/customer/{customer.CustomerId}"));
        }

        // PATCH: /Customers/5/verified
        [HttpPatch("{id}/verified", Name = "UpdateCustomerVerified")]
        public IActionResult UpdateVerified(uint id, [FromBody] bool verified)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            customer.Verified = verified;

            return Ok(new PostReference(1, $"{BasePath}/customer/{customer.CustomerId}"));
        }

        // PATCH: /Customers/5/chipcarduid
        [HttpPatch("{id}/chipcarduid", Name = "UpdateCustomerChipCardUid")]
        public IActionResult UpdateChipCardUid(uint id, [FromBody] string chipCardUid)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbCustomer customer = Context.Customers.Find(id);
            if (customer == null)
                return NotFound(new Error(201, "Customer with requested id does not exist.",
                    $"There is no customer that has the id {id}."));

            customer.ChipCardUid = chipCardUid;

            return Ok(new PostReference(1, $"{BasePath}/customer/{customer.CustomerId}"));
        }

        // GET: /Customers/by-lastname/5
        [HttpGet("by-lastname/{name}", Name = "GetCustomerByLastName")]
        public IActionResult GetCustomerByLastName(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            List<DbCustomer> customers = Context.Customers.Where(c => c.LastName == name).ToList();
            if (customers.Count == 0)
                return NoContent();

            return Ok(customers);
        }
    }
}
