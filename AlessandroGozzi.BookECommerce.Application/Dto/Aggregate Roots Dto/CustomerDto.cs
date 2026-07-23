using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto
{
    public record CustomerDto(
        Guid CustomerId,
        string Name,
        string Surname,
        string Email,
        string PhoneNumber,
        AddressDto Address,
        string TaxCode
        )
    { }
}
