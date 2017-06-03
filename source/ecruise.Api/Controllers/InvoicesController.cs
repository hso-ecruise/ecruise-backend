using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;

using ecruise.Models;
using DbInvoice = ecruise.Database.Models.Invoice;
using DbInvoiceItem = ecruise.Database.Models.InvoiceItem;
using System.Linq;
using ecruise.Models.Assemblers;

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

            return Ok(InvoiceAssembler.AssembleModelList(invoices));
        }

        // GET: /invoices/1
        [HttpGet("{id}", Name = "GetInvoiceByInvoiceId")]
        public IActionResult Get(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = Context.Invoices.Find(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            else
                return Ok(InvoiceAssembler.AssembleModel(invoice));
        }

        // GET: /invoices/by-invoice-item/1
        [HttpGet("by-invoice-item/{id}", Name = "GetInvoiceByInvoiceItemId")]
        public IActionResult GetByInvoiceItemId(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoiceItem item = Context.InvoiceItems.Find(id);

            if(item == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            return Ok(InvoiceAssembler.AssembleModel(item.Invoice));
        }

        // GET: /invoices/by-customer/{customerId}
        [HttpGet("by-customer/{customerId}", Name = "GetInvoiceByCustomerId")]
        public IActionResult GetInvoiceByCustomerId(ulong customerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            ImmutableList<DbInvoice> invoices = Context.Invoices
                .Where(t => t.CustomerId == customerId)
                .ToImmutableList();

            if (invoices.Count == 0)
                return NoContent();

            return Ok(InvoiceAssembler.AssembleModelList(invoices));
        }

        // PATCH: /invoices/1/paid
        [HttpPatch("{id}/paid")]
        public IActionResult Patch(ulong id, [FromBody] bool paid)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = Context.Invoices.Find(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            invoice.Paid = paid;

            Context.SaveChanges();

            return Ok(new PostReference(id, $"{BasePath}/invoices/{id}"));
        }

        // GET: /invoices/1/items
        [HttpGet("{id}/items", Name = "GetAllInvoiceItems")]
        public IActionResult GetAllInvoiceItems(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));
            
            DbInvoice invoice = Context.Invoices.Find(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            ImmutableList<DbInvoiceItem> items = invoice.InvoiceItem.ToImmutableList();

            if(items.Count == 0)
                return NoContent();

            return Ok(InvoiceItemAssembler.AssembleModelList(items));
        }

        // POST: /Invoices/1/items
        [HttpPost("{id}/items", Name = "CreateNewInvoiceItem")]
        public IActionResult Post(ulong id, [FromBody] InvoiceItem invoiceItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = Context.Invoices.Find(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            DbInvoiceItem insertItem = InvoiceItemAssembler.AssembleEntity(0, invoiceItem);

            var inserted = Context.InvoiceItems.Add(insertItem);
            Context.SaveChanges();

            return Created($"{BasePath}/invoices/{inserted.Entity.InvoiceId}",
                new PostReference((uint)inserted.Entity.InvoiceItemId, $"{BasePath}/invoices/{inserted.Entity.InvoiceId}"));
        }

        // GET: /invoices/items/1
        [HttpGet("items/{invoiceItemId}", Name = "GetInvoiceItem")]
        public IActionResult GetInvoiceItem(ulong invoiceItemId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoiceItem invoiceItem = Context.InvoiceItems.Find(invoiceItemId);

            if (invoiceItem == null)
                return NotFound(new Error(201, "InvoiceItem with requested id does not exist.",
                    $"There is no invoice item that has the id {invoiceItemId}."));
            
            return Ok(InvoiceItemAssembler.AssembleModel(invoiceItem));
        }
    }
}
