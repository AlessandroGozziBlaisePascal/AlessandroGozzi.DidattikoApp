using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;

namespace AlessandroGozzi.BookECommerce.Application
{
    public interface IPaymentService
    {
        Task<(bool,CreditCard)> IsPaymentSuccessful(string paymentIntentId, CancellationToken cancellationToken = default);
    }
}
