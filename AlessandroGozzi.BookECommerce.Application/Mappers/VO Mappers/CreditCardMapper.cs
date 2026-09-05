using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class CreditCardMapper
    {
        public static CreditCardDto ToDto(this CreditCard card)
        {
            return new CreditCardDto(
                Owner: card.CardOwner.ToString(),
                Last4Digits: card.DisplayName,
                ExpiryDate: card.ExpiryDate.ToDto()
            );
        }
    }
}
