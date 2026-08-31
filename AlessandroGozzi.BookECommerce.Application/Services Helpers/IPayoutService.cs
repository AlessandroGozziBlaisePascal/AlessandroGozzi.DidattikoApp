using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi.BookECommerce.Application.Services_Helpers
{
    public interface IPayoutService
    {
        Task<Result> SendPayoutAsync(Guid customerId, decimal amount, string iban, CancellationToken token = default);
    }
}
