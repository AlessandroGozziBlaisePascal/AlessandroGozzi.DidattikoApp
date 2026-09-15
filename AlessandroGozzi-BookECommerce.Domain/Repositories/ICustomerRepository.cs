using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers;

namespace AlessandroGozzi.BookECommerce.Domain.Repositories
{
    public interface ICustomerRepository: IRepository<Customer>
    {
        Task<Customer?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
        Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    }
}
