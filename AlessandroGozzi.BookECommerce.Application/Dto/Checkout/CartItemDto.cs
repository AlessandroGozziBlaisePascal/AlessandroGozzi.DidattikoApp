using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
{
    public record CartItemDto(
    Guid CartItemId,            
    Guid BookId,               
    string ISBN,
    string Title,
    string Condition,        
    string MainPhoto,
    decimal UnitPrice,         
    int Quantity,              
    decimal Subtotal,         
    bool IsStillAvailable,     
    string SellerName,
    string SellerSurname
    )
    { }

}
