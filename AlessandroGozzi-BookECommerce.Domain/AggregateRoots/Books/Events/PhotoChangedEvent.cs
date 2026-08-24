using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books.ValueObjects;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books.Events
{
    public record PhotoChangedEvent(Guid BookId, ImageUrl ImageUrl) : DomainEvent { }
}
