using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class CartItemMapper
    {
        public static CartItemDto ToDto(this CartItem item, Book book)
        {
            return new CartItemDto(
                item.Id,
                item.BookId,
                item.BookTitle,
                item.MainPhoto,
                item.Price.ToDto(),
                item.Quantity,
                book.IsAvailable
            );
        }
    }
}
