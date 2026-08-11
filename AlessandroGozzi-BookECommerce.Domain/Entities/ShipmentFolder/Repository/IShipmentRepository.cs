using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Repository
{
    public interface IShipmentRepository
    {
        void Add(Shipment shipment);
        Task<Shipment?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<List<Shipment>> GetByOrderIdAsync(Guid orderId, CancellationToken token = default);
        Task<List<Shipment>> GetBySellerIdAsync(Guid sellerId, CancellationToken token = default);
    }
}
