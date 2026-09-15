using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books.ValueObjects;

namespace AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books.Events
{
    public record ReviewAddedEvent(Guid BookId, BookReview Review) : DomainEvent { }
}
