using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Carts;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Wallets;

namespace AlessandroGozzi.BookECommerce.Domain.Repositories
{
    public interface IWalletRepository: IRepository<Wallet>
    {
        Task<Wallet?> GetByCustomerId(Guid customerId, CancellationToken cancellationToken = default);
    }
}
