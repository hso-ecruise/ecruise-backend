using System.Collections.Generic;
using System.Linq;

using CustomerToken = ecruise.Models.CustomerToken;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;

namespace ecruise.Models.Assemblers
{
    public class CustomerTokenAssembler
    {
        public static DbCustomerToken AssembleEntity(CustomerToken customerTokenModel)
        {
            return new DbCustomerToken
            {
                CustomerTokenId = customerTokenModel.CustomerTokenId,
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
                customerTokenEntity.CustomerTokenId,
                customerTokenEntity.CustomerId,
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

        public static List<DbCustomerToken> AssembleEntityList(IList<CustomerToken> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}
