using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace ecruise.Models.Assemblers
{
    public class CustomerTokenAssembler
    {
        public static Database.Models.CustomerToken AssembleEntity(CustomerToken customerTokenModel)
        {
            return new Database.Models.CustomerToken
            {
                CustomerTokenId = customerTokenModel.CustomerTokenId,
                CustomerId = customerTokenModel.CustomerId,
                Type = (Database.Models.TokenType)customerTokenModel.Type,
                Token = customerTokenModel.Token,
                CreationDate = customerTokenModel.CreationDate,
                ExpireDate = customerTokenModel.ExpireDate
            };
        }

        public static CustomerToken AssembleModel(Database.Models.CustomerToken customerTokenEntity)
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
    }
}
