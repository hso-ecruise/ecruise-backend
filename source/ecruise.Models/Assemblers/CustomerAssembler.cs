using System;
using System.Collections.Generic;
using System.Text;

namespace ecruise.Models.Assemblers
{
    public class CustomerAssembler
    {
        public static Database.Models.Customer AssembleEntity(Customer customerModel)
        {
            return new Database.Models.Customer
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

        public static Customer AssembleModel(Database.Models.Customer customerEntity)
        {
            return new Customer(
                (uint)customerEntity.CustomerId,
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
    }
}
