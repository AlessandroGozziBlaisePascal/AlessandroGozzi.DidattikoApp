using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;

namespace AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.Events
{
    public record NameChangedEvent(Guid CustomerId, FullName OldName, FullName NewName) : DomainEvent { }
}
