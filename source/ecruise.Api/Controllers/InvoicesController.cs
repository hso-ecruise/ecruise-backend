using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ecruise.Models;

namespace ecruise.Api.Controllers
{
    public class InvoicesController : BaseController
    {
        // GET: /Invoices
        [HttpGet(Name = "GetAllInvoices")]
        public IActionResult Get()
        {
            Invoice invoice1 = new Invoice(1, 123.45, false);
            Invoice invoice2 = new Invoice(1, 0.27, true);

            return Ok(new List<Invoice> { invoice1, invoice2 });
        }
        
        // POST: /Invoices
        [HttpPost(Name = "CreateNewInvoice")]
        public IActionResult Post([FromBody] Invoice invoice1)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/invoices/1",
                    new PostReference(invoice1.InvoiceId, $"{BasePath}/invoices/1"));
            else
                return BadRequest(new Error(1, ModelState.ToString(),
                    "An error occured. Please check the message for further information."));
        }

        // GET: /invoices/1
        [HttpGet("{id}", Name = "GetInvoice")]
        public IActionResult Get(uint id)
        {
            if (ModelState.IsValid && id < 3)
            {
                Invoice invoice1 = new Invoice(1, 123.45, false);
                return Ok(invoice1);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound(new Error(1, "Invoice with requested Invoice id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // PATCH: /invoices/1/paid
        [HttpPatch("{id}/paid")]
        public IActionResult Patch(uint id, [FromBody] string date)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Created($"{BasePath}/invoices/{id}",
                    new PostReference(id, $"{BasePath}/invoices/{id}"));
            }
            else if (ModelState.IsValid && id >= 3)
            {
                return NotFound(new Error(1, "Invoice with requested Invoice does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /invoices/1/items
        [HttpGet("{id}/items", Name = "GetAllInvoiceItems")]
        public IActionResult GetAllInvoiceItems(uint id)
        {
            InvoiceItem item1 = new InvoiceItem(1, 1, "Trip123", InvoiceItem.TypeEnum.Credit, 10.0);
            InvoiceItem item2 = new InvoiceItem(1, 1, "MwSt", InvoiceItem.TypeEnum.Credit, 1.9);

            return Ok(new List<InvoiceItem> { item1, item2 });
        }

        // POST: /Invoices/1/items
        [HttpPost("{id}/itemsName", Name = "CreateNewInvoiceItem")]
        public IActionResult Post(uint id, [FromBody] Invoice invoice1)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/invoices/1",
                    new PostReference(invoice1.InvoiceId, $"{BasePath}/invoices/1"));
            else
                return BadRequest(new Error(1, ModelState.ToString(),
                    "An error occured. Please check the message for further information."));
        }

        // GET: /invoices/1/items/1
        [HttpGet("{id}/items/{itemId}", Name = "GetInvoiceItem")]
        public IActionResult GetAllInvoiceItems(uint id, uint itemId, [FromBody] Invoice invoice1)
        {
            if (ModelState.IsValid && id < 3)
            {
                InvoiceItem item1 = new InvoiceItem(1, 1, "Trip123", InvoiceItem.TypeEnum.Credit, 10.0);
                return Ok(item1);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound(new Error(1, "Invoice with requested Invoice id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }
    }
}