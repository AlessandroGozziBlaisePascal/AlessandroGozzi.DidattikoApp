using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.Book.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.Book.Event
{
    public record ReviewAddedEvent(Guid BookId, BookReview Review) : DomainEvent { }
}
