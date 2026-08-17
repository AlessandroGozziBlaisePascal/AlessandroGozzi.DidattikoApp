using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.Repository;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository
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
    }
}
