using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(this Customer customer) =>
            new CustomerDto(
                customer.Id,
                customer.FullName.ToString(),
                customer.Email.Value,
                customer.Number.Value,
                customer.Address.ToDto()
            );
    }
}
