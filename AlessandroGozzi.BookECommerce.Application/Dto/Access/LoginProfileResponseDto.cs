using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Access
{
    public record LoginProfileResponseDto(
        CustomerDto customer,
        bool HasSavedCreditCard,
        string? MaskedCardNumbers
        ) { }
}
