using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Orders;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyCollection<Order>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken token = default)
        {
            var orders = await Context.Orders
                .Include("_items")
                .Include("_shipmentIds")
                .Where(o => o.CustomerId == customerId)
                .ToListAsync(token);

            return orders.AsReadOnly();
        }
    }

}
