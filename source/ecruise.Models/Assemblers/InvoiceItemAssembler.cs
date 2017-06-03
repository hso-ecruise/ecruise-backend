using System;
using System.Collections.Generic;
using System.Linq;

using InvoiceItem = ecruise.Models.InvoiceItem;
using DbInvoiceItem = ecruise.Database.Models.InvoiceItem;

namespace ecruise.Models.Assemblers
{
    public static class InvoiceItemAssembler
    {
        public static string EnumToString(InvoiceItem.TypeEnum t)
        {
            switch (t)
            {
                case InvoiceItem.TypeEnum.Credit:
                    return "CREDIT";
                case InvoiceItem.TypeEnum.Debit:
                    return "DEBIT";
                default:
                    throw new NotImplementedException();
            }
        }

        public static InvoiceItem.TypeEnum StringToEnum(string e)
        {
            switch (e)
            {
                case "CREDIT":
                    return InvoiceItem.TypeEnum.Credit;
                case "DEBIT":
                    return InvoiceItem.TypeEnum.Debit;
                default:
                    throw new NotImplementedException();
            }
        }

        public static DbInvoiceItem AssembleEntity(ulong invoiceItemId, InvoiceItem invoiceItemModel)
        {
            return new DbInvoiceItem
            {
                InvoiceItemId = invoiceItemId != 0 ? invoiceItemId : invoiceItemModel.InvoiceItemId,
                InvoiceId = invoiceItemModel.InvoiceId,
                Reason = invoiceItemModel.Reason,
                Type = EnumToString(invoiceItemModel.Type),
                Amount = invoiceItemModel.Amount
            };
        }

        public static InvoiceItem AssembleModel(DbInvoiceItem invoiceItemEntity)
        {
            return new InvoiceItem(
                (uint)invoiceItemEntity.InvoiceItemId,
                (uint?)invoiceItemEntity.InvoiceId,
                invoiceItemEntity.Reason,
                StringToEnum(invoiceItemEntity.Type),
                invoiceItemEntity.Amount);
        }

        public static List<InvoiceItem> AssembleModelList(IList<DbInvoiceItem> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbInvoiceItem> AssembleEntityList(bool setIdsNull, IList<InvoiceItem> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            else
                return models.Select(e => AssembleEntity(e.InvoiceItemId, e)).ToList();
        }
    }
}
