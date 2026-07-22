using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Access
{
    public record LoginProfileResponseDto(
        Guid CustomerId, 
        string Name, 
        string Surname, 
        string Email, 
        string PhoneNumber, 
        bool HasSavedCreditCard,
        string? MaskedCardNumbers
        ) { }
}
