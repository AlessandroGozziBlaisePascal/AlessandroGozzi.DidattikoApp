using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.CustomerEvent
{
    public record ProfileUpdatedEvent(Guid CustomerId, List<string> UpdatedFields): DomainEvent;
}
