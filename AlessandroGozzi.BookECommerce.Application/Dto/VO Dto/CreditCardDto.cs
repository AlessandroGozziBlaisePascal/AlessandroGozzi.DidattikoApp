using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto
{
    public record CreditCardDto(
    string OwnerName,
    string OwnerSurname,
    string Last4Digits, 
    string ExpiryDate
    );
}
