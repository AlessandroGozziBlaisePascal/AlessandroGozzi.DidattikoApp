using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Orders;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers
{
    public static class OrderItemMapper
    {
        public static OrderItemDto ToDto(this OrderItem item) => new OrderItemDto(
                item.Id,
                item.BookId,
                item.BookTitle,
                item.Price.Amount,
                item.MainPhoto.Value,
                item.Quantity,
                item.SellerId
        );
    }
}

