using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            var cleanIdentifier = identifier.Trim();

            return await Context.Customers
                .FirstOrDefaultAsync(c =>
                    c.Email.Value == cleanIdentifier ||
                    c.Number.Value == cleanIdentifier,
                    cancellationToken);
        }
    }
}
