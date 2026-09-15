using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Orders;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers
{
    public static class OrderMapper
    {
        public static OrderDto ToDto(this Order order)
        {
            var itemDtos = order.Items.Select(item => item.ToDto()).ToList().AsReadOnly();

            return new OrderDto(
                OrderId: order.Id,
                BuyerId: order.CustomerId,
                PlacedAt: order.Date,
                Status: order.Status.ToString(),
                TotalAmount: order.TotalPrice.Amount,
                PaymentDetails: order.PaymentDetails?.ToDto(),
                Items: itemDtos
            );
        }
    }
}
