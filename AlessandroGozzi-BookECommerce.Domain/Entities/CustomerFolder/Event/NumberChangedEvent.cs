using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Event
{
    public record NumberChangedEvent(Guid CustomerId, PhoneNumber OldNumber, PhoneNumber NewNumber) : DomainEvent { }
}
