using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi.BookECommerce.Application
{
    public interface IPaymentService
    {
        Task<Result<PaymentDetailsDto>> IsPaymentSuccessfulAsync(string paymentIntentId, CancellationToken cancellationToken = default);

        Task<Result<CheckoutPaymentResultDto>> CreatePaymentIntentAsync(
            decimal amount,
            Guid customerId,
            Guid cartId,
            CancellationToken cancellationToken = default,
            string currency = "eur"
            );
    }
}
