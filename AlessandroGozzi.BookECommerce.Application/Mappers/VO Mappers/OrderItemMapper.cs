using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class OrderItemMapper
    {
        public static OrderItemDto ToDto(this OrderItem item, Book book, Customer seller) => new OrderItemDto(
                item.Id,
                item.BookId,
                book.ISBNCode.ToDto(),
                book.Title,
                book.Status.ToDto(),
                item.Price.ToDto(),
                SellerId: seller.Id,
                SellerFullName: $"{seller.Name.Value} {seller.Surname.Value}"
        );
    }
}

