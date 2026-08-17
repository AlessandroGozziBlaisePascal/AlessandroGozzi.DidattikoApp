using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.Repository;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Repository
{
    public interface IOrderRepository: IRepository<Order>
    {
        Task<Order?> GetByPaymentIntentIdAsync(string paymenIntentId, CancellationToken token = default);
        Task<IReadOnlyCollection<Order>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken token = default);
    }
}
