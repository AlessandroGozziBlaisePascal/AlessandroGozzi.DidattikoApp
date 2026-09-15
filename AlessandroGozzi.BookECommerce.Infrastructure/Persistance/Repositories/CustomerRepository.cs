using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
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

        public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            // Cerca se l'entità è già presente e tracciata nel Change Tracker di EF Core
            var trackedEntity = Context.Customers.Local.FirstOrDefault(c => c.Id == customer.Id);

            if (trackedEntity == null)
            {
                // Se non è tracciata in memoria, la colleghiamo/aggiorniamo
                Context.Customers.Update(customer);
            }
            else if (!ReferenceEquals(trackedEntity, customer))
            {
                // Se c'è un'istanza diversa già tracciata con lo stesso ID, aggiorniamo i valori
                Context.Entry(trackedEntity).CurrentValues.SetValues(customer);
            }

            // Se l'istanza è esattamente la stessa (ReferenceEquals == true), 
            // EF Core la sta già tracciando ed è già a conoscenza delle modifiche.
            return Task.CompletedTask;
        }
    }
}
