using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.Order.Repository
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<IReadOnlyList<Order>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken token = default);
        Task AddAsync(Order order, CancellationToken token = default);
        Task UpdateAsync(Order order, CancellationToken token = default);
    }
}
