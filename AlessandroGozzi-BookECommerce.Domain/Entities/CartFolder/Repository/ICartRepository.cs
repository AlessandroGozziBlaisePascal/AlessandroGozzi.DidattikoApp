using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.Repository;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository
{
    public interface ICartRepository: IRepository<Cart>
    {
        Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken token = default);
    }
}
