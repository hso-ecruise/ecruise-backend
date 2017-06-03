using System.Collections.Generic;
using System.Linq;

using InvoiceItem = ecruise.Models.InvoiceItem;
using DbInvoiceItem = ecruise.Database.Models.InvoiceItem;

namespace ecruise.Models.Assemblers
{
    public class InvoiceItemAssembler
    {
        public static DbInvoiceItem AssembleEntity(InvoiceItem invoiceItemModel)
        {
            return new DbInvoiceItem
            {
                InvoiceItemId = invoiceItemModel.InvoiceItemId,
                InvoiceId = invoiceItemModel.InvoiceId,
                Reason = invoiceItemModel.Reason,
                Type = (Database.Models.InvoiceItemType)invoiceItemModel.Type,
                Amount = invoiceItemModel.Amount
            };
        }

        public static InvoiceItem AssembleModel(DbInvoiceItem invoiceItemEntity)
        {
            return new InvoiceItem(
                invoiceItemEntity.InvoiceItemId,
                invoiceItemEntity.InvoiceId,
                invoiceItemEntity.Reason,
                (InvoiceItem.TypeEnum)invoiceItemEntity.Type,
                invoiceItemEntity.Amount);
        }

        public static List<InvoiceItem> AssembleModelList(IList<DbInvoiceItem> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbInvoiceItem> AssembleEntityList(IList<InvoiceItem> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}
