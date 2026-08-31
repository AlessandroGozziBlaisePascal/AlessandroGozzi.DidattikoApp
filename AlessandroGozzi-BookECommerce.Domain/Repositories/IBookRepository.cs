using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books;

namespace AlessandroGozzi_BookECommerce.Domain.Repositories
{
    public interface IBookRepository: IRepository<Book>
    {
        Task<Book?> GetByISBNAsync(string ISBN, CancellationToken cancellationToken = default);
        Task<IEnumerable<Book>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Book>> GetByBooksDetailsAsync(
            string? title = null,
            string? publisher = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? condition = null,
            string? subject = null,
            CancellationToken cancellationToken = default
        );
        Task<Dictionary<string, int>> GetAvailableSellersCountByIsbnsAsync(
            List<string> isbns,
            CancellationToken cancellationToken
        );
    }
}
