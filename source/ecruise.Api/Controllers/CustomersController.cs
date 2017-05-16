using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ecruise.Models;

namespace ecruise.Api.Controllers
{
    public class CustomersController : BaseController
    {
        // TODO: Requires to be admin
        // GET: /Customers
        [HttpGet(Name = "GetAllCustomers")]
        public IActionResult GetAll()
        {
            Customer c1 = new Customer(1, "admin@ecruise.me", "078108151", "", "Peter", "Admin", "DE", "Offenburg",
                77652, "Badstraﬂe", "24a", "", false, false);
            Customer c2 = new Customer(2, "tschwendemann@ecruise.me", "078108152", "", "Tobias", "Schwendemann", "DE",
                "Offenburg", 77652, "Badstraﬂe", "24a", "", true, true);
            Customer c3 = new Customer(3, "ffischbach@ecruise.me", "078108153", "", "Felix", "Fischbach", "DE",
                "Offenburg", 77652, "Badstraﬂe", "24a", "", false, true);
            Customer c4 = new Customer(4, "mhauer@ecruise.me", "078108154", "", "Moritz", "Hauer", "DE", "Offenburg",
                77652, "Badstraﬂe", "24a", "", true, false);

            return Ok(new List<Customer> {c1, c2, c3});
        }

        // TODO: Customer can only request it's own data \
        //         Admins can view all customers.
        // GET: /Customers/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult GetOne(uint id)
        {
            if (ModelState.IsValid && (id > 0 && id < 3))
            {
                Customer c = new Customer(1, "admin@ecruise.me", "072210815", "", "Peter", "Admin", "DE", "Offenburg",
                    77652, "Badstraﬂe", "24a", "", false, false);

                return Ok(c);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound(new Error(1, "Customer with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            }
        }

        // POST: /Customers
        [HttpPost(Name = "CreateCustomer")]
        public IActionResult Post([FromBody] Registration r)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/customers/1",
                    new PostReference(1, $"{BasePath}/customers/1"));
            else
            {
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            }
        }

        // PATCH: /Customers/5/password
        [HttpPatch("{id}/password", Name = "UpdateCustomerPassword")]
        public IActionResult UpdatePassword(uint id, [FromBody] string password)
        {
            if (ModelState.IsValid && (id < 3 && id > 0))
            {
                return Created($"{BasePath}/customers/{id}",
                    new PostReference(id, $"{BasePath}/customers/{id}"));
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "Customer with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            }
        }

        // PATCH: /Customers/5/email
        [HttpPatch("{id}/email", Name = "UpdateCustomerEmail")]
        public IActionResult UpdateEmail(uint id, [FromBody] string email)
        {
            if (ModelState.IsValid && (id < 3 && id > 0))
            {
                return Created($"{BasePath}/customers/{id}",
                    new PostReference(id, $"{BasePath}/customers/{id}"));
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "Customer with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            }
        }

        // PATCH: /Customers/5/phone-number
        [HttpPatch("{id}/phone-number", Name = "UpdateCustomerPhoneNumber")]
        public IActionResult UpdatePhoneNumber(uint id, [FromBody] string phone)
        {
            if (ModelState.IsValid && (id < 3 && id > 0))
            {
                return Created($"{BasePath}/customers/{id}",
                    new PostReference(id, $"{BasePath}/customers/{id}"));
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "Customer with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            }
        }

        // PATCH: /Customers/5/address
        [HttpPatch("{id}/address", Name = "UpdateCustomerAddress")]
        public IActionResult UpdateAddress(uint id, [FromBody] Address address)
        {
            if (ModelState.IsValid && (id < 3 && id > 0))
            {
                return Created($"{BasePath}/customers/{id}",
                    new PostReference(id, $"{BasePath}/customers/{id}"));
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "Customer with requested id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /Customers/by-lastname/5
        [HttpGet("by-lastname/{name}", Name = "GetCustomerByLastName")]
        public IActionResult GetCustomerByLastName(string name)
        {
            Customer c1 = new Customer(1, "peter@ecruise.me", "072210815", "", "Peter", "Mustermann", "DE", "Offenburg",
                77652, "Badstraﬂe", "24a", "", false, false);
            Customer c2 = new Customer(7, "andrea@ecruise.me", "072210815", "", "Andrea", "Mustermann", "DE", "Offenburg",
                77652, "Badstraﬂe", "24a", "", false, false);

            return Ok(new List<Customer>{c1, c2});
        }
    }
}
