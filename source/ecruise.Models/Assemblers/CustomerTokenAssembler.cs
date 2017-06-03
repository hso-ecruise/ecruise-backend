using System;
using System.Collections.Generic;
using System.Linq;
using CustomerToken = ecruise.Models.CustomerToken;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;

namespace ecruise.Models.Assemblers
{
    public class CustomerTokenAssembler
    {
        private static string makeStringFromEnum(ecruise.Models.CustomerToken.TokenTypeEnum t)
        {
            switch (t)
            {
                case CustomerToken.TokenTypeEnum.EmailActivation:
                    return "EMAIL_ACTIVATION";
                case CustomerToken.TokenTypeEnum.Login:
                    return "LOGIN";
                default:
                    throw new NotImplementedException();
            }
        }

        private static CustomerToken.TokenTypeEnum makeEnumFromString(string e)
        {
            switch (e)
            {
                case "EMAIL_ACTIVATION":
                    return CustomerToken.TokenTypeEnum.EmailActivation;
                case "LOGIN":
                    return CustomerToken.TokenTypeEnum.Login;
                default:
                    throw new NotImplementedException();
            }  
        }

        public static DbCustomerToken AssembleEntity(ulong customerTokenId, CustomerToken customerTokenModel)
        {
            return new DbCustomerToken
            {
                CustomerTokenId = customerTokenId != 0 ? customerTokenId : customerTokenModel.CustomerTokenId,
                CustomerId = customerTokenModel.CustomerId,
                Type = makeStringFromEnum(customerTokenModel.Type),
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
                makeEnumFromString(customerTokenEntity.Type),
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
