using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.Application.Dto.Checkout
{
    public record PaymentDetailsDto(
        bool IsSuccess,
        string PaymentIntentId,
        string CardBrand,
        string CardLast4Digits
        );
}
