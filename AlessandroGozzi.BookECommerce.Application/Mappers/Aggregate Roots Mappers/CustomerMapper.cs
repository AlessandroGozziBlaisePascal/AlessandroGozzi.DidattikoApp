using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Access;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(this Customer customer) =>
            new CustomerDto(
                customer.Id,
                customer.Name.Value,
                customer.Surname.Value,
                customer.Email.Value,
                customer.Number.Value,
                customer.Address.ToDto(),
                customer.TaxCode.ToDto()
            );
        public static Customer ToDomain(this CustomerDto customerDto) =>
            Customer.Create(
                customerDto.Name.ToNameDomain(),
                customerDto.Surname.ToSurnameDomain(),
                customerDto.Email.ToEmailDomain(),
                customerDto.Address.ToAddressDomain(),
                customerDto.PhoneNumber.ToNumberDomain(),
                customerDto.TaxCode.ToTaxCodeDomain(),

            ).Value;
    }
}
