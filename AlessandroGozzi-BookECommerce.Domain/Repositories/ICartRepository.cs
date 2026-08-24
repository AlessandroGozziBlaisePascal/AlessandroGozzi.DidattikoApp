using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Carts;

namespace AlessandroGozzi_BookECommerce.Domain.Repositories
{
    public interface ICartRepository: IRepository<Cart>
    {
        Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken token = default);
    }
}
