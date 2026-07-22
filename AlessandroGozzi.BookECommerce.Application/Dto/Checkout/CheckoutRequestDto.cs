using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
{
    public record CheckoutRequestDto(
    Guid CustomerId,
    Guid CartId,
    string? ShippingAddress = null,      
    string? PaymentMethod = null,         
    string? PaymentToken = null
    )
    { }

}
