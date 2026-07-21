using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Event
{
    public record BookRemovedFromCartEvent(Guid CustomerId, Guid CartId, Guid BookId, int Quantity) : DomainEvent { }
}
