using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers
{
    public static class CartMapper
    {
        public static CartDto ToDto(this Cart cart, IEnumerable<Book> books)
        {
            var bookDict = books.ToDictionary(b => b.Id);
            var itemDtos = cart.GetItems
           .Select(item => item.ToDto(bookDict[item.BookId]))
           .ToList();

            return new CartDto(
            cart.Id,
            cart.CustomerId,
            (IReadOnlyCollection<Dto.VO_Dto.CartItemDto>)itemDtos,
            itemDtos.Sum(x => x.Subtotal),  
            itemDtos.Sum(x => x.Quantity)    
            );
        }
       
    }
}
