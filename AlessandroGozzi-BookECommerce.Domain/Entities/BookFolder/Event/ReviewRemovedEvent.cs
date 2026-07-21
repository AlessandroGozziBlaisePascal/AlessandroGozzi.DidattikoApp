using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Event
{
    public record ReviewRemovedEvent(Guid BookId, BookReview Review) : DomainEvent { }
}
