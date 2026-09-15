using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments;

namespace AlessandroGozzi.BookECommerce.Domain.Repositories
{
    public interface IShipmentRepository: IRepository<Shipment>
    {
        Task<List<Shipment>> GetByOrderIdAsync(Guid orderId, CancellationToken token = default);
        Task<List<Shipment>> GetBySellerIdAsync(Guid sellerId, CancellationToken token = default);
    }
}
