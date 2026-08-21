using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.Repository;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository
{
    public interface ICustomerRepository: IRepository<Customer>
    {
        Task<Customer?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    }
}
