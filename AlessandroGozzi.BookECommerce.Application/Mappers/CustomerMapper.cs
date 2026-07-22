using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Access;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers
{
    public static class CustomerMapper
    {
        public static LoginProfileResponseDto ToDto(this Customer customer) =>
            new LoginProfileResponseDto(
                customer.Id,
                customer.Name.Value,
                customer.Surname.Value,
                customer.Email.Value,
                customer.Number.Value,
                customer.CreditCard != null,
                customer.CreditCard.DisplayName
            );
    }
}
