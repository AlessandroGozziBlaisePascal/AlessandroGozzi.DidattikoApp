using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
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
