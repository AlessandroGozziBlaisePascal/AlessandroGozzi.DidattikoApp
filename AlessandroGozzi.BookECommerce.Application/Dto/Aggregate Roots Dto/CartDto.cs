using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto
{
    public record CartDto(
        Guid CartId,
        Guid CustomerId,
        IReadOnlyCollection<CartItemDto> Items,
        decimal TotalAmount,
        int TotalItemsCount
    )
    { }

}
