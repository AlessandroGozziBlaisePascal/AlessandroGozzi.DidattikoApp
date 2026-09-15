using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Carts;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken token = default)
        {
            return await Context.Carts
                .Include("_items")
                .FirstOrDefaultAsync(c => c.CustomerId == customerId, token);
        }
    }

}
