using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Persistance.Repositories
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot
    {
        protected readonly ApplicationDbContext Context;

        public Repository(ApplicationDbContext context)
        {
            Context = context;
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await Context.Set<T>().FindAsync(new object[] { id }, token);
        }

        public virtual async Task<List<T>> GetAllAsync(CancellationToken token = default)
        {
            return await Context.Set<T>().ToListAsync(token);
        }

        public virtual async Task<List<T>> GetByIdsAsync(List<Guid> ids, CancellationToken token = default)
        {
            return await Context.Set<T>()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(token);
        }

        public virtual void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public virtual void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
        }
    }

}
