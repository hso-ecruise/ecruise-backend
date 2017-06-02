using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    class InvoiceItemAssembler
    {
        public static Database.Models.InvoiceItem AssembleEntity(InvoiceItem invoiceItemModel)
        {
            return new Database.Models.InvoiceItem
            {
                InvoiceItemId = invoiceItemModel.InvoiceItemId,
                InvoiceId = invoiceItemModel.InvoiceId,
                Reason = invoiceItemModel.Reason,
                Type = (Database.Models.InvoiceItemType)invoiceItemModel.Type,
                Amount = invoiceItemModel.Amount
            };
        }

        public static InvoiceItem AssembleModel(Database.Models.InvoiceItem invoiceItemEntity)
        {
            return new InvoiceItem(
                invoiceItemEntity.InvoiceItemId,
                invoiceItemEntity.InvoiceId,
                invoiceItemEntity.Reason,
                (InvoiceItem.TypeEnum)invoiceItemEntity.Type,
                invoiceItemEntity.Amount);
        }
    }
}
