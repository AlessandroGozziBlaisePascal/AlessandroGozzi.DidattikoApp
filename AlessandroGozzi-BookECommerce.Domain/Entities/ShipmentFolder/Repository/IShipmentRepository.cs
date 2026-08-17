using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.Repository;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Repository
{
    public interface IShipmentRepository: IRepository<Shipment>
    {
        Task<List<Shipment>> GetByOrderIdAsync(Guid orderId, CancellationToken token = default);
        Task<List<Shipment>> GetBySellerIdAsync(Guid sellerId, CancellationToken token = default);
    }
}
