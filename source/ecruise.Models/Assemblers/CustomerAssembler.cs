﻿using System.Collections.Generic;
using System.Linq;

using Customer = ecruise.Models.Customer;
using DbCustomer = ecruise.Database.Models.Customer;

namespace ecruise.Models.Assemblers
{
    public class CustomerAssembler
    {
        public static DbCustomer AssembleEntity(Customer customerModel)
        {
            return new DbCustomer
            {
                CustomerId = customerModel.CustomerId,
                Email = customerModel.Email,
                PhoneNumber = customerModel.PhoneNumber,
                ChipCardUid = customerModel.ChipCardUid,
                FirstName = customerModel.FirstName,
                LastName = customerModel.LastName,
                Country = customerModel.Country,
                City = customerModel.City,
                ZipCode = customerModel.ZipCode,
                Street = customerModel.Street,
                HouseNumber = customerModel.HouseNumber,
                AddressExtraLine = customerModel.AddressExtraLine,
                Activated = customerModel.Activated,
                Verified = customerModel.Verified
            };
        }

        public static Customer AssembleModel(DbCustomer customerEntity)
        {
            return new Customer(
                customerEntity.CustomerId,
                customerEntity.Email,
                customerEntity.PhoneNumber,
                customerEntity.ChipCardUid,
                customerEntity.FirstName,
                customerEntity.LastName,
                customerEntity.Country,
                customerEntity.City,
                customerEntity.ZipCode,
                customerEntity.Street,
                customerEntity.HouseNumber,
                customerEntity.AddressExtraLine,
                customerEntity.Activated,
                customerEntity.Verified);
        }

        public static List<Customer> AssembleModelList(IList<DbCustomer> entities)
        {
            return entities.Select(AssembleModel).ToList();
        }

        public static List<DbCustomer> AssembleEntityList(IList<Customer> models)
        {
            return models.Select(AssembleEntity).ToList();
        }
    }
}
