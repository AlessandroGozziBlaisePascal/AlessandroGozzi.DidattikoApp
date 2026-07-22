using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
{
    public record OrderDto(
        Guid OrderId,
        string OrderCode,                
        Guid BuyerId,
        DateTime PlacedAt,               
        string Status,                    
        IReadOnlyCollection<OrderItemDto> Items,
        decimal TotalAmount,
        string PaymentTransactionId,     
        ShippingAddressDto ShippingAddress
    );

}
