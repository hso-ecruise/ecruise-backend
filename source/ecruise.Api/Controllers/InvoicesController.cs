using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecruise.Database.Models;
using ecruise.Models;
using ecruise.Models.Assemblers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbInvoice = ecruise.Database.Models.Invoice;
using DbInvoiceItem = ecruise.Database.Models.InvoiceItem;
using InvoiceItem = ecruise.Models.InvoiceItem;

namespace ecruise.Api.Controllers
{
    public class InvoicesController : BaseController
    {
        public InvoicesController(EcruiseContext context) : base(context)
        {
        }

        // GET: /Invoices
        [HttpGet(Name = "GetAllInvoicesAsync")]
        public async Task<IActionResult> GetAllInvoicesAsync()
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
        [HttpGet("{id}", Name = "GetInvoiceByInvoiceIdAsync")]
        public async Task<IActionResult> GetAsync(ulong id)
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
                return Unauthorized();

            return Ok(InvoiceAssembler.AssembleModel(invoice));
        }

        // GET: /invoices/by-invoice-item/1
        [HttpGet("by-invoice-item/{id}", Name = "GetInvoiceByInvoiceItemIdAsync")]
        public async Task<IActionResult> GetByInvoiceItemIdAsync(ulong id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoiceItem item = await Context.InvoiceItems.FindAsync(id);

            if (item == null)
                return NotFound(new Error(201, "Invoice-Item with requested id does not exist.",
                    $"There is no invoice that has the id {id}."));

            // Get matching invoice from database
            var invoice = await Context.Invoices.FindAsync(item.InvoiceItemId);

            // forbid if current customer is accessing a different user's invoice
            if (!HasAccess(invoice.CustomerId))
                return Unauthorized();

            return Ok(InvoiceAssembler.AssembleModel(invoice));
        }

        // GET: /invoices/by-customer/{customerId}
        [HttpGet("by-customer/{customerId}", Name = "GetInvoiceByCustomerIdAsync")]
        public async Task<IActionResult> GetInvoiceByCustomerIdAsync(ulong customerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            // forbid if current customer is accessing a different user's invoices
            if (!HasAccess(customerId))
                return Unauthorized();

            List<DbInvoice> invoices = await Context.Invoices
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();

            if (invoices.Count == 0)
                return NoContent();

            return Ok(InvoiceAssembler.AssembleModelList(invoices));
        }

        // PATCH: /invoices/1/paid
        [HttpPatch("{id}/paid", Name = "UpdateInvoiceAsync")]
        public async Task<IActionResult> PatchAsync(ulong id, [FromBody] bool paid)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

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
        [HttpGet("{id}/items", Name = "GetAllInvoiceItemsAsync")]
        public async Task<IActionResult> GetAllInvoiceItemsAsync(ulong id)
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
                return Unauthorized();

            List<DbInvoiceItem> items = await Context.InvoiceItems
                .Where(i => i.InvoiceId == id)
                .ToListAsync();

            if (items.Count == 0)
                return NoContent();

            return Ok(InvoiceItemAssembler.AssembleModelList(items));
        }

        // POST: /Invoices/1/items
        [HttpPost("{id}/items", Name = "CreateNewInvoiceItemAsync")]
        public async Task<IActionResult> PostAsync(ulong id, [FromBody] InvoiceItem invoiceItem)
        {
            // forbid if not admin
            if (!HasAccess())
                return Unauthorized();

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
                switch (invoiceItem.Type)
                {
                    case InvoiceItem.TypeEnum.Debit:
                        invoice.TotalAmount += insertItem.Amount;
                        break;
                    case InvoiceItem.TypeEnum.Credit:
                        invoice.TotalAmount -= insertItem.Amount;
                        break;
                    default:
                        return BadRequest(new Error(400, "The provided invoice item type is invalid",
                            "An error occured. Please check the message for further information."));
                }

                transaction.Commit();
                await Context.SaveChangesAsync();

                return Created($"{BasePath}/invoices/{inserted.Entity.InvoiceId}",
                    new PostReference((uint)inserted.Entity.InvoiceItemId,
                        $"{BasePath}/invoices/{inserted.Entity.InvoiceId}"));
            }
        }

        // GET: /invoices/items/1
        [HttpGet("items/{invoiceItemId}", Name = "GetInvoiceItemAsync")]
        public async Task<IActionResult> GetInvoiceItemAsync(ulong invoiceItemId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error(400, GetModelStateErrorString(),
                    "An error occured. Please check the message for further information."));

            DbInvoiceItem invoiceItem = await Context.InvoiceItems.FindAsync(invoiceItemId);

            if (invoiceItem == null)
                return NotFound(new Error(201, "InvoiceItem with requested id does not exist.",
                    $"There is no invoice item that has the id {invoiceItemId}."));

            // forbid if current customer is accessing a different user's invoice item
            // Get matching invoice
            var invoice = await Context.Invoices.FindAsync(invoiceItem.InvoiceId);

            if (!HasAccess(invoice.CustomerId))
                return Unauthorized();

            return Ok(InvoiceItemAssembler.AssembleModel(invoiceItem));
        }
    }
}
