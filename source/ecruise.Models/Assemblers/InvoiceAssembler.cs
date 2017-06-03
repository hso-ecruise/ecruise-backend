using System.Collections.Generic;
using System.Linq;

using Invoice = ecruise.Models.Invoice;
using DbInvoice = ecruise.Database.Models.Invoice;

namespace ecruise.Models.Assemblers
{
    public class InvoiceAssembler
    {
        public static DbInvoice AssembleEntity(ulong invoiceId, Invoice invoiceModel)
        {
            return new DbInvoice
            {
                InvoiceId = invoiceId != 0 ? invoiceId : invoiceModel.InvoiceId,
                CustomerId = invoiceModel.CustomerId,
                TotalAmount = invoiceModel.TotalAmount,
                Paid = invoiceModel.Paid
            };
        }

        public static Invoice AssembleModel(DbInvoice invoiceEntity)
        {
            return new Invoice(
                (uint)invoiceEntity.InvoiceId,
                (uint)invoiceEntity.CustomerId,
                invoiceEntity.TotalAmount,
                invoiceEntity.Paid);
        }

        public static List<Invoice> AssembleModelList(IList<DbInvoice> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbInvoice> AssembleEntityList(bool setIdsNull, IList<Invoice> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            else
                return models.Select(e => AssembleEntity(e.InvoiceId, e)).ToList();
        }
    }
}
