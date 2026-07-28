using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Repository
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<Order?> GetByPaymentIntentIdAsync(string paymenIntentId, CancellationToken token = default);
        Task<IReadOnlyCollection<Order>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken token = default);
        Task AddAsync(Order order, CancellationToken token = default);
        Task UpdateAsync(Order order, CancellationToken token = default);
    }
}
