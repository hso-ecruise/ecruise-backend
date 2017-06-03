using System.Collections.Generic;
using System.Linq;

using Invoice = ecruise.Models.Invoice;
using DbInvoice = ecruise.Database.Models.Invoice;

namespace ecruise.Models.Assemblers
{
    public class InvoiceAssembler
    {
        public static DbInvoice AssembleEntity(Invoice invoiceModel)
        {
            return new DbInvoice
            {
                InvoiceId = invoiceModel.InvoiceId,
                CustomerId = invoiceModel.CustomerId,
                TotalAmount = invoiceModel.TotalAmount,
                Payed = invoiceModel.Paid
            };
        }

        public static Invoice AssembleModel(DbInvoice invoiceEntity)
        {
            return new Invoice(
                invoiceEntity.InvoiceId,
                invoiceEntity.CustomerId,
                invoiceEntity.TotalAmount,
                invoiceEntity.Payed);
        }

        public static List<Invoice> AssembleModelList(IList<DbInvoice> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbInvoice> AssembleEntityList(IList<Invoice> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}
