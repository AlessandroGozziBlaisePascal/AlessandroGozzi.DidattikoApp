using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto
{
    public record OrderItemDto(
        Guid OrderItemId,
        Guid BookId,
        string ISBN,
        string Title,
        string Condition,
        decimal PriceAtPurchase,
        Guid SellerId,
        string SellerFullName
    );

}
