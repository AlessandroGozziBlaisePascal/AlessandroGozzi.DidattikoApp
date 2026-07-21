using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.CreditcCardGestion
{
    public record CreditCardResponseDto(string OwnerName, string OwnerSurname, string Last4Digits, string ExpiryDate) { }
}
