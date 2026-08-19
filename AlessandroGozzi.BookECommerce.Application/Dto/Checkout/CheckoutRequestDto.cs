using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
{
    public record CheckoutRequestDto(
    Guid CartId,
    AddressDto? ShippingAddress = null,      
    string? PaymentMethod = null,         
    string? PaymentToken = null
    )
    { }

}
