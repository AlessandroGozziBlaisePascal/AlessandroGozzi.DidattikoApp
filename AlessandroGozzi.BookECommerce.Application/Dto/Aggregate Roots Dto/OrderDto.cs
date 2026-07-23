using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto
{
    public record OrderDto(
        Guid OrderId,
        string OrderCode,
        Guid BuyerId,
        DateTime PlacedAt,
        string Status,
        decimal TotalAmount,
        string? PaymentTransactionId,
        IReadOnlyCollection<OrderItemDto> Items
    );
}
