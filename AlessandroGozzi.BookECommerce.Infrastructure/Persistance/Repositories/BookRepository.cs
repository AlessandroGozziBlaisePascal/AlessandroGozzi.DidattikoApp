using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Book?> GetByISBNAsync(string ISBN, CancellationToken cancellationToken = default)
        {
            return await Context.Books
                .FirstOrDefaultAsync(b => b.ISBNCode.Value == ISBN, cancellationToken);
        }

        public async Task<IEnumerable<Book>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await Context.Books
                .Where(b => b.SellerId == customerId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Book>> GetByBooksDetailsAsync(
            string? title = null,
            string? publisher = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? condition = null,
            string? subject = null,
            CancellationToken cancellationToken = default)
        {
            var query = Context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(b => b.Price.Amount >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price.Amount <= maxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(subject))
            {
                query = query.Where(b => b.Subject.Value.Contains(subject));
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<string, int>> GetAvailableSellersCountByIsbnsAsync(
            List<string> isbns,
            CancellationToken cancellationToken = default)
        {
            return await Context.Books
                .Where(b => isbns.Contains(b.ISBNCode.Value) && b.IsAvailable)
                .GroupBy(b => b.ISBNCode.Value)
                .Select(g => new { ISBN = g.Key, Count = g.Select(b => b.SellerId).Distinct().Count() })
                .ToDictionaryAsync(x => x.ISBN, x => x.Count, cancellationToken);
        }
    }

}
