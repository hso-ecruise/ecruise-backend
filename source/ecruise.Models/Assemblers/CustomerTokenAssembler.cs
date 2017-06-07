using System;
using System.Collections.Generic;
using System.Linq;
using DbCustomerToken = ecruise.Database.Models.CustomerToken;

namespace ecruise.Models.Assemblers
{
    public static class CustomerTokenAssembler
    {
        public static string EnumToString(CustomerToken.TokenTypeEnum t)
        {
            switch (t)
            {
                case CustomerToken.TokenTypeEnum.EmailActivation:
                    return "EMAIL_ACTIVATION";
                case CustomerToken.TokenTypeEnum.Login:
                    return "LOGIN";
                case CustomerToken.TokenTypeEnum.EmailChangePhase1:
                    return "EMAIL_CHANGE_PHASE_1";
                case CustomerToken.TokenTypeEnum.EmailChangePhase2:
                    return "EMAIL_CHANGE_PHASE_2";
                default:
                    throw new NotImplementedException();
            }
        }

        public static CustomerToken.TokenTypeEnum StringToEnum(string e)
        {
            switch (e)
            {
                case "EMAIL_ACTIVATION":
                    return CustomerToken.TokenTypeEnum.EmailActivation;
                case "LOGIN":
                    return CustomerToken.TokenTypeEnum.Login;
                case "EMAIL_CHANGE_PHASE_1":
                    return CustomerToken.TokenTypeEnum.EmailChangePhase1;
                case "EMAIL_CHANGE_PHASE_2":
                    return CustomerToken.TokenTypeEnum.EmailChangePhase2;
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
                Type = EnumToString(customerTokenModel.Type),
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
                StringToEnum(customerTokenEntity.Type),
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

            return models.Select(e => AssembleEntity(e.CustomerTokenId, e)).ToList();
        }
    }
}
