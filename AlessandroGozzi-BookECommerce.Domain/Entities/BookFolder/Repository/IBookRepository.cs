using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Book?> GetByISBNAsync(string ISBN, CancellationToken cancellationToken = default);
        Task<IEnumerable<Book>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Book>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task<IEnumerable<Book>> GetByBooksDetailsAsync(string subject, int schoolYear, string? title = null, CancellationToken cancellationToken = default);
        Task AddAsync (Book book, CancellationToken cancellationToken = default);
        Task UpdateAsync (Book book, CancellationToken cancellationToken = default);
        Task DeleteAsync (Book book, CancellationToken cancellationToken = default);
    }
}
