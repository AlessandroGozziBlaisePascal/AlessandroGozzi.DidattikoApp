using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Orders;

namespace AlessandroGozzi_BookECommerce.Domain.Repositories
{
    public interface IOrderRepository: IRepository<Order>
    {
        Task<Order?> GetByPaymentIntentIdAsync(string paymenIntentId, CancellationToken token = default);
        Task<IReadOnlyCollection<Order>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken token = default);
    }
}
