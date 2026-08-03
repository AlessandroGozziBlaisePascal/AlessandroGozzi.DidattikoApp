using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class CreditCardMapper
    {
        public static CreditCard ToCreditCardDomain(this CreditCardDto dto)
        {
            return CreditCard.Create(dto.)
        }
    }
}
