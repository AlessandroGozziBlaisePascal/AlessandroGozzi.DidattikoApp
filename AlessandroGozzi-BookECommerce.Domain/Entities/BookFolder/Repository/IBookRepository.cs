using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync (Book book, CancellationToken cancellationToken = default);
        Task UpdateAsync (Book book, CancellationToken cancellationToken = default);
        Task DeleteAsync (Book book, CancellationToken cancellationToken = default);
    }
}
