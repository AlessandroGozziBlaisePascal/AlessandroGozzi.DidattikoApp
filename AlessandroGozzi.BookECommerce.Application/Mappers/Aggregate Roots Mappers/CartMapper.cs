using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Carts;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers
{
    public static class CartMapper
    {
        public static CartDto ToDto(this Cart cart, IEnumerable<Book> books)
        {
            var bookDict = books.ToDictionary(b => b.Id);

            var itemDtos = cart.GetItems.Select(item =>
            {
                bookDict.TryGetValue(item.BookId, out var book);
                return item.ToDto(book);
            }).ToList().AsReadOnly();

            return new CartDto(
                cart.Id,
                cart.CustomerId,
                itemDtos,
                itemDtos.Sum(x => x.UnitPrice),
                itemDtos.Sum(x => x.Quantity));
        }
       
    }
}
