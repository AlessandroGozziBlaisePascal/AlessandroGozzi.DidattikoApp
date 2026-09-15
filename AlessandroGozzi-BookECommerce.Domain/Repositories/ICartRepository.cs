using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Carts;

namespace AlessandroGozzi.BookECommerce.Domain.Repositories
{
    public interface ICartRepository: IRepository<Cart>
    {
        Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken token = default);
    }
}
