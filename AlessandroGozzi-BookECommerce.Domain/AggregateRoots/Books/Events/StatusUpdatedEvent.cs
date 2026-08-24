using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books.Events
{
    public record StatusUpdatedEvent(Guid BookId, BookStatus OldStatus, BookStatus NewStatus) : DomainEvent { }
}
