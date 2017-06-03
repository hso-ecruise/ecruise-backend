using System.Collections.Immutable;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
using DbInvoice = ecruise.Database.Models.Invoice;
using DbInvoiceItem = ecruise.Database.Models.InvoiceItem;
using System.Linq;

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
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = Context.Invoices.Find(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));
            else
                return Ok(invoice);
        }

        // GET: /invoices/by-invoice-item/1
        [HttpGet("by-invoice-item/{id}", Name = "GetInvoiceByInvoiceItemId")]
        public IActionResult GetByInvoiceItemId(uint id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoiceItem item = Context.InvoiceItems.Find(id);
            if(item == null)
                return NotFound(new Error(201, "Trip with requested id does not exist.",
                    $"There is no trip that has the id {id}."));
            return Ok(item.Invoice);
        }

        // GET: /invoices/by-customer/{customerId}
        [HttpGet("by-customer/{customerId}", Name = "GetInvoiceByCustomerId")]
        public IActionResult GetInvoiceByCustomerId(uint customerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            ImmutableList<DbInvoice> invoices = Context.Invoices
                .Where(t => t.CustomerId == customerId)
                .ToImmutableList();

            if (invoices.Count == 0)
                return NoContent();

            return Ok(invoices);
        }

        // PATCH: /invoices/1/paid
        [HttpPatch("{id}/paid")]
        public IActionResult Patch(uint id, [FromBody] bool paid)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = Context.Invoices.Find(id);
            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no trip that has the id {id}."));

            using (var transaction = Context.Database.BeginTransaction())
            {
                invoice.Payed = paid;
                transaction.Commit();
            }

            return Ok(new PostReference(id, $"{BasePath}/invoices/{id}"));
        }

        // GET: /invoices/1/items
        [HttpGet("{id}/items", Name = "GetAllInvoiceItems")]
        public IActionResult GetAllInvoiceItems(uint id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            
            DbInvoice invoice = Context.Invoices.Find(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no maintenance that has the id {id}."));

            ImmutableList<DbInvoiceItem> items = invoice.InvoiceItem.ToImmutableList();

            if(items.Count == 0)
                return NoContent();

            return Ok(items);
        }

        // POST: /Invoices/1/items
        [HttpPost("{id}/items", Name = "CreateNewInvoiceItem")]
        public IActionResult Post(uint id, [FromBody] InvoiceItem invoiceItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = Context.Invoices.Find(id);
            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no trip that has the id {id}."));

            DbInvoiceItem insertItem = new DbInvoiceItem
            {
                InvoiceItemId = invoiceItem.InvoiceItemId,
                InvoiceId = invoiceItem.InvoiceId,
                Reason = invoiceItem.Reason,
                Type = (ecruise.Database.Models.InvoiceItemType)invoiceItem.Type,
                Amount = invoiceItem.Amount,
                Invoice = invoice,
            };

            var inserted = Context.InvoiceItems.Add(insertItem);

            using (var transaction = Context.Database.BeginTransaction())
            {
                invoice.InvoiceItem.Add(insertItem);
                transaction.Commit();
            }

            return Created($"{BasePath}/Invoices/{inserted.Entity.InvoiceId}",
                new PostReference((uint)inserted.Entity.InvoiceItemId, $"{BasePath}/Invoices/{inserted.Entity.InvoiceId}"));
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
