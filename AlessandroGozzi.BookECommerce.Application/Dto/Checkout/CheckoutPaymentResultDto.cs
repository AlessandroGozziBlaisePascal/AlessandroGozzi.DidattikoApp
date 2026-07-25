using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
{
    public record CheckoutPaymentResultDto(
        string ClientSecret,
        string PaymentIntentId,
        long Amount,
        string Currency = "eur"
    );
}
