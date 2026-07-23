using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
{
    public record BankPaymentRequestDto(
    string TransactionId,   
    decimal Amount,        
    string Currency,        
    string PaymentToken,   
    string CustomerEmail,   
    string Description     
);

}
