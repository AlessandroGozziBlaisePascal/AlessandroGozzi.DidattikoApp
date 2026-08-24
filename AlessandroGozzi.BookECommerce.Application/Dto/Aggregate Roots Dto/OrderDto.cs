using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto
{
    public record OrderDto(
        Guid OrderId,
        Guid BuyerId,
        DateTime PlacedAt,
        string Status,
        decimal TotalAmount,
        PaymentDetailsDto PaymentDetails,
        IReadOnlyCollection<OrderItemDto> Items
    );
}
