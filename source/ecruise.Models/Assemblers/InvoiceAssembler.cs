using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    class InvoiceAssembler
    {
        public static Database.Models.Invoice AssembleEntity(Invoice invoiceModel)
        {
            return new Database.Models.Invoice
            {
                InvoiceId = invoiceModel.InvoiceId,
                CustomerId = invoiceModel.CustomerId,
                TotalAmount = invoiceModel.TotalAmount,
                Payed = invoiceModel.Paid
            };
        }

        public static Invoice AssembleModel(Database.Models.Invoice invoiceEntity)
        {
            return new Invoice(
                invoiceEntity.InvoiceId,
                invoiceEntity.CustomerId,
                invoiceEntity.TotalAmount,
                invoiceEntity.Payed);
        }
    }
}
