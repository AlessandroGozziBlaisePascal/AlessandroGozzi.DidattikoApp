using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments;

namespace AlessandroGozzi_BookECommerce.Domain.Repositories
{
    public interface IShipmentRepository: IRepository<Shipment>
    {
        Task<List<Shipment>> GetByOrderIdAsync(Guid orderId, CancellationToken token = default);
        Task<List<Shipment>> GetBySellerIdAsync(Guid sellerId, CancellationToken token = default);
    }
}
