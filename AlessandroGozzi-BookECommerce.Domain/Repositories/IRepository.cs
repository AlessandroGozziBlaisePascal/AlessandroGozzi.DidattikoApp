using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Repositories
{
    public interface IRepository<T> where T : AggregateRoot
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<List<T>> GetAllAsync(CancellationToken token = default);
        Task<List<T>> GetByIdsAsync(List<Guid> ids, CancellationToken token = default);

        void Remove(T entity); 
        void Add(T entity);
    }
}
