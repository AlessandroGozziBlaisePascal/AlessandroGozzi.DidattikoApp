using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto
{
    public record CartItemDto(
    Guid CartItemId,            
    Guid BookId,  
    Guid SellerId,
    string Title,        
    string MainPhoto,
    decimal UnitPrice,         
    int Quantity,         
    bool IsStillAvailable
    )
    { }

}
