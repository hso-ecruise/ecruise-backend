using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    public class InvoiceItemAssembler
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
                (uint)invoiceItemEntity.InvoiceItemId,
                (uint?)invoiceItemEntity.InvoiceId,
                invoiceItemEntity.Reason,
                (InvoiceItem.TypeEnum)invoiceItemEntity.Type,
                invoiceItemEntity.Amount);
        }
    }
}
