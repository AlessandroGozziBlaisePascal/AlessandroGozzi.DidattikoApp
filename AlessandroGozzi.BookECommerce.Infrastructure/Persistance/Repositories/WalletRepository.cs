using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Wallets;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Repositories
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Wallet?> GetByCustomerId(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await Context.Wallets
                .FirstOrDefaultAsync(w => w.CustomerId == customerId, cancellationToken);
        }
    }

}
