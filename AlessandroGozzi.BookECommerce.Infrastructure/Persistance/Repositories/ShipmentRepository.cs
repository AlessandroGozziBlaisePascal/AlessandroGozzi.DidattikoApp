using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Repositories
{
    public class ShipmentRepository : Repository<Shipment>, IShipmentRepository
    {
        public ShipmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Shipment>> GetByOrderIdAsync(Guid orderId, CancellationToken token = default)
        {
            return await Context.Shipments
                .Where(s => s.OrderId == orderId)
                .ToListAsync(token);
        }

        public async Task<List<Shipment>> GetBySellerIdAsync(Guid sellerId, CancellationToken token = default)
        {
            return await Context.Shipments
                .Where(s => s.VendorId == sellerId)
                .ToListAsync(token);
        }
    }

}
