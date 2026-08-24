using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Carts.Events
{
    public record BookAddedToCartEvent(Guid CustomerId, Guid CartId, Guid BookId) : DomainEvent { }
}
