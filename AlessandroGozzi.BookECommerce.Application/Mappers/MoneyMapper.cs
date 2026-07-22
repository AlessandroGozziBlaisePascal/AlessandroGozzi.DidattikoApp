using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities;

namespace AlessandroGozzi.BookECommerce.Application.Mappers
{
    public static class MoneyMapper
    {
        public static decimal ToDto(this Money money) => money.Amount;

        public static Money ToDomain(this decimal price) => Money.Create(price).Value;
    }
}
