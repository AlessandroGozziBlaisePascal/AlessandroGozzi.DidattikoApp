using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository
{
    public interface ICartRepository
    {
        Task<Cart?> GetByIdAsync(Guid id, CancellationToken token);
        Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken token = default);
        Task AddAsync(Cart cart, CancellationToken token = default);
        Task UpdateAsync(Cart cart, CancellationToken token = default);
        Task DeleteAsync(Cart cart, CancellationToken token = default);
    }
}
