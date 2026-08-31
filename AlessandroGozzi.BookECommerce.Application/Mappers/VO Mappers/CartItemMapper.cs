using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Carts;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class CartItemMapper
    {
        public static CartItemDto ToDto(this CartItem item, Book book)
        {
            return new CartItemDto(
                item.BookId,
                item.SellerId,
                item.BookTitle,
                item.MainPhoto.Value,
                item.Price.Amount,
                item.Quantity,
                book.IsAvailable
            );
        }
    }
}
