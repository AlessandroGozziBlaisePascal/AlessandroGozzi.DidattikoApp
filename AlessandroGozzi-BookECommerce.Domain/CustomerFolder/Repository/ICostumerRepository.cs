using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Repository
{
    public interface ICostumerRepository
    {
        Task<Result<Customer>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<Customer>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<Result> AddAsync(Customer customer, CancellationToken cancellationToken = default);
        Result Update(Customer customer);
        Result Delete(Customer customer);
    }
}
