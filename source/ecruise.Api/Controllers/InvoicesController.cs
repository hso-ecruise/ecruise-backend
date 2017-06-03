using System.Collections.Immutable;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
using DbInvoice = ecruise.Database.Models.Invoice;

namespace ecruise.Api.Controllers
{
    public class InvoicesController : BaseController
    {
        // GET: /Invoices
        [HttpGet(Name = "GetAllInvoices")]
        public IActionResult Get()
        {
            ImmutableList<DbInvoice> invoices = Context.Invoices.ToImmutableList();

            if (invoices.Count == 0)
                return NoContent();
            return Ok(invoices);
        }

        // GET: /invoices/1
        [HttpGet("{id}", Name = "GetInvoiceByInvoiceId")]
        public IActionResult Get(uint id)
        {
            if (ModelState.IsValid && id < 3)
            {
                Invoice invoice1 = new Invoice(1, 1, 123.45, false);
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

        // GET: /invoices/by-invoice-item/1
        [HttpGet("by-invoice-item/{id}", Name = "GetInvoiceByInvoiceItemId")]
        public IActionResult GetByInvoiceItemId(uint id)
        {
            if (ModelState.IsValid && id < 3)
            {
                Invoice invoice1 = new Invoice(1, 1, 123.45, false);
                return Ok(invoice1);
            }
            else if (ModelState.IsValid && (id >= 3 || id == 0))
            {
                return NotFound(new Error(1, "Invoice with requested Invoice-Item-id does not exist.",
                    "An error occured. Please check the message for further information."));
            }
            else
            {
                return BadRequest(new Error(1, "The id given was not formatted correctly. Id has to be unsinged int",
                    "An error occured. Please check the message for further information."));
            }
        }

        // GET: /invoices/by-customer/{customerId}
        [HttpGet("by-customer/{customerId}", Name = "GetInvoiceByCustomerId")]
        public IActionResult GetInvoiceByCustomerId(uint customerId)
        {
            if (ModelState.IsValid && customerId < 3)
            {
                Invoice i1 = new Invoice(1, customerId, 12.34, false);
                Invoice i2 = new Invoice(1, customerId, 56.78, true);
                return Ok(new List<Invoice> { i1, i2 });
            }
            else if (ModelState.IsValid && (customerId >= 3 || customerId == 0))
            {
                return NotFound(new Error(1, "No customerId.",
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
        public IActionResult Patch(uint id, [FromBody] bool paid)
        {
            if (ModelState.IsValid && id < 3 && id > 0)
            {
                return Ok(new PostReference(id, $"{BasePath}/invoices/{id}"));
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

            return Ok(new List<InvoiceItem> {item1, item2});
        }

        // POST: /Invoices/1/items
        [HttpPost("{id}/items", Name = "CreateNewInvoiceItem")]
        public IActionResult Post(uint id, [FromBody] InvoiceItem invoiceItem)
        {
            if (ModelState.IsValid)
                return Created($"{BasePath}/invoices/by-invoice-item/{invoiceItem.InvoiceItemId}",
                    new PostReference(invoiceItem.InvoiceItemId,
                        $"{BasePath}/invoices/by-invoice-item/{invoiceItem.InvoiceItemId}"));
            else
                return BadRequest(new Error(1, ModelState.ToString(),
                    "An error occured. Please check the message for further information."));
        }

        // GET: /invoices/1/items/1
        [HttpGet("{id}/items/{invoiceItemId}", Name = "GetInvoiceItem")]
        public IActionResult GetAllInvoiceItems(uint id, uint invoiceItemId)
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
