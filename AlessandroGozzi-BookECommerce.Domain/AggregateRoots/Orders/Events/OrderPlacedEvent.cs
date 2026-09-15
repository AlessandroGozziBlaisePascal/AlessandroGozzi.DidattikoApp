using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Orders.Events
{
    public record OrderPlacedEvent(Guid OrderId): DomainEvent { }
}
