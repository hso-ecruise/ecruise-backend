using System.Collections.Generic;
using System.Linq;

using CustomerToken = ecruise.Models.CustomerToken;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;

namespace ecruise.Models.Assemblers
{
    public class CustomerTokenAssembler
    {
        public static DbCustomerToken AssembleEntity(ulong customerTokenId, CustomerToken customerTokenModel)
        {
            return new DbCustomerToken
            {
                CustomerTokenId = customerTokenId != 0 ? customerTokenId : customerTokenModel.CustomerTokenId,
                CustomerId = customerTokenModel.CustomerId,
                Type = (Database.Models.TokenType)customerTokenModel.Type,
                Token = customerTokenModel.Token,
                CreationDate = customerTokenModel.CreationDate,
                ExpireDate = customerTokenModel.ExpireDate
            };
        }

        public static CustomerToken AssembleModel(DbCustomerToken customerTokenEntity)
        {
            return new CustomerToken(
                (uint)customerTokenEntity.CustomerTokenId,
                (uint)customerTokenEntity.CustomerId,
                (CustomerToken.TokenTypeEnum)customerTokenEntity.Type,
                customerTokenEntity.Token,
                customerTokenEntity.CreationDate,
                customerTokenEntity.ExpireDate
                );
        }

        public static List<CustomerToken> AssembleModelList(IList<DbCustomerToken> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbCustomerToken> AssembleEntityList(bool setIdsNull, IList<CustomerToken> models)
        {
            if (setIdsNull)
                return models.Select(e => AssembleEntity(0, e)).ToList();

            else
                return models.Select(e => AssembleEntity(e.CustomerTokenId, e)).ToList();
        }
    }
}
