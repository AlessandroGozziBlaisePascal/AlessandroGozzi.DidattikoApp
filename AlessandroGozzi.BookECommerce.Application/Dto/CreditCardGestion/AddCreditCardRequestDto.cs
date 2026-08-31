using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.CreditCardGestion
{
    public record AddCreditCardRequestDto(string CardOwnerName, string CardOwnerSurname, string CardNumber, string ExpiryDate)
    {
        public override string ToString() => $"AddCreditCardRequestDto  {{ Owner = {CardOwnerName} {CardOwnerSurname}, Expiry = {ExpiryDate}, CardNumber = [PROTECTED]}}";

    }
}
