using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers;

namespace AlessandroGozzi_BookECommerce.Domain.Repositories
{
    public interface ICustomerRepository: IRepository<Customer>
    {
        Task<Customer?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
        Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    }
}
