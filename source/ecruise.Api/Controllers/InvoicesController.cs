using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbInvoice = ecruise.Database.Models.Invoice;
using DbInvoiceItem = ecruise.Database.Models.InvoiceItem;

namespace ecruise.Api.Controllers
{
    public class InvoicesController : BaseController
    {
        // GET: /Invoices
        [HttpGet(Name = "GetAllInvoices")]
        public async Task<IActionResult> Get()
        {
            List<DbInvoice> invoices = await Context.Invoices
                // query only invoices the current customer has access to
                .Where(i => HasAccess(i.CustomerId))
                .ToListAsync();

            if (invoices.Count == 0)
                return NoContent();

            return Ok(InvoiceAssembler.AssembleModelList(invoices));
        }

        // GET: /invoices/1
        [HttpGet("{id}", Name = "GetInvoiceByInvoiceId")]
        public async Task<IActionResult> Get(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = await Context.Invoices.FindAsync(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            // forbid if current customer is accessing a different user's invoice
            if (!HasAccess(invoice.CustomerId))
                return Forbid();

            return Ok(InvoiceAssembler.AssembleModel(invoice));
        }

        // GET: /invoices/by-invoice-item/1
        [HttpGet("by-invoice-item/{id}", Name = "GetInvoiceByInvoiceItemId")]
        public async Task<IActionResult> GetByInvoiceItemId(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoiceItem item = await Context.InvoiceItems.FindAsync(id);

            if (item == null)
                return NotFound(new Error(201, "Invoice-Item with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            // forbid if current customer is accessing a different user's invoice
            if (!HasAccess(item.Invoice.CustomerId))
                return Forbid();

            return Ok(InvoiceAssembler.AssembleModel(item.Invoice));
        }

        // GET: /invoices/by-customer/{customerId}
        [HttpGet("by-customer/{customerId}", Name = "GetInvoiceByCustomerId")]
        public async Task<IActionResult> GetInvoiceByCustomerId(ulong customerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // forbid if current customer is accessing a different user's invoices
            if (!HasAccess(customerId))
                return Forbid();

            List<DbInvoice> invoices = await Context.Invoices
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();

            if (invoices.Count == 0)
                return NoContent();

            return Ok(InvoiceAssembler.AssembleModelList(invoices));
        }

        // PATCH: /invoices/1/paid
        [HttpPatch("{id}/paid")]
        public async Task<IActionResult> Patch(ulong id, [FromBody] bool paid)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = await Context.Invoices.FindAsync(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            invoice.Paid = paid;
            await Context.SaveChangesAsync();

            return Ok(new PostReference(id, $"{BasePath}/invoices/{id}"));
        }

        // GET: /invoices/1/items
        [HttpGet("{id}/items", Name = "GetAllInvoiceItems")]
        public async Task<IActionResult> GetAllInvoiceItems(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = await Context.Invoices.FindAsync(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            // forbid if current customer is accessing a different user's invoices
            if (!HasAccess(invoice.CustomerId))
                return Forbid();

            List<DbInvoiceItem> items = await Context.InvoiceItems
                .Where(i => i.InvoiceId == id)
                .ToListAsync();

            if (items.Count == 0)
                return NoContent();

            return Ok(InvoiceItemAssembler.AssembleModelList(items));
        }

        // POST: /Invoices
        [HttpPost(Name = "CreateInvoice")]
        public async Task<IActionResult> PostInvoice([FromBody] Invoice invoice)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice insertInvoice = InvoiceAssembler.AssembleEntity(0, invoice);

            var inserted = await Context.Invoices.AddAsync(insertInvoice);
            await Context.SaveChangesAsync();

            return Created($"{BasePath}/invoices/{inserted.Entity.InvoiceId}",
                new PostReference((uint)inserted.Entity.InvoiceId, $"{BasePath}/invoices/{inserted.Entity.InvoiceId}"));
        }

        // POST: /Invoices/1/items
        [HttpPost("{id}/items", Name = "CreateNewInvoiceItem")]
        public async Task<IActionResult> Post(ulong id, [FromBody] InvoiceItem invoiceItem)
        {
            // forbid if not admin
            if (!HasAccess())
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoice invoice = await Context.Invoices.FindAsync(id);

            if (invoice == null)
                return NotFound(new Error(201, "Invoice with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            if (invoice.Paid)
                return StatusCode(StatusCodes.Status409Conflict);

            DbInvoiceItem insertItem = InvoiceItemAssembler.AssembleEntity(0, invoiceItem);

            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                // insert invoice item
                var inserted = await Context.InvoiceItems.AddAsync(insertItem);

                // update invoice total amount
                Context.Invoices.Find(insertItem.InvoiceId).TotalAmount += insertItem.Amount;

                transaction.Commit();
                await Context.SaveChangesAsync();

                return Created($"{BasePath}/invoices/{inserted.Entity.InvoiceId}",
                    new PostReference((uint)inserted.Entity.InvoiceItemId,
                        $"{BasePath}/invoices/{inserted.Entity.InvoiceId}"));
            }
        }

        // GET: /invoices/items/1
        [HttpGet("items/{invoiceItemId}", Name = "GetInvoiceItem")]
        public async Task<IActionResult> GetInvoiceItem(ulong invoiceItemId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoiceItem invoiceItem = await Context.InvoiceItems.FindAsync(invoiceItemId);

            if (invoiceItem == null)
                return NotFound(new Error(201, "InvoiceItem with requested id does not exist.",
                    $"There is no invoice item that has the id {invoiceItemId}."));

            // forbid if current customer is accessing a different user's invoice item
            if (!HasAccess(invoiceItem.Invoice.CustomerId))
                return Forbid();

            return Ok(InvoiceItemAssembler.AssembleModel(invoiceItem));
        }
    }
}
