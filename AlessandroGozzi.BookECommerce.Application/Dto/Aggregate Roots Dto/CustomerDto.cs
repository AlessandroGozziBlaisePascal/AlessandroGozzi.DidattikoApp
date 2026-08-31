using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto
{
    public record CustomerDto(
        Guid CustomerId,
        string FullName,
        string Email,
        string PhoneNumber,
        AddressDto Address
        )
    { }
}
