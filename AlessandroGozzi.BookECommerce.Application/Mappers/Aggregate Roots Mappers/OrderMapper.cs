using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder;

namespace AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers
{
    public static class OrderMapper
    {
        public static OrderDto ToDto(this Order order, IReadOnlyCollection<Book> books, IReadOnlyCollection<Customer> sellers)
        {
            var bookDict = books.ToDictionary(b => b.Id);
            var sellerDict = sellers.ToDictionary(s => s.Id);

            var itemDtos = order.Items
                .Select(item =>
                {
                    var book = bookDict[item.BookId];
                    var seller = sellerDict[book.SellerId]; 

                    return item.ToDto(book, seller);
                })
                .ToList()
                .AsReadOnly();

            return new OrderDto(
                OrderId: order.Id,
                OrderCode: order.TrackingCode,
                BuyerId: order.CustomerId,
                PlacedAt: order.Date,
                Status: order.Status.ToString(),
                TotalAmount: order.TotalPrice.Amount,
                PaymentTransactionId: order.PaymentDetails?.PaymentToken, 
                Items: itemDtos
            );
        }
    }
}
