using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
{
    public record PaymentResponseDto(
    bool IsSuccess,
    string TransactionId,        
    Guid? OrderId,            
    string Status,            
    string? ErrorMessage,       
    DateTime ProcessedAt
    )
    { }

}
