using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.Customer.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.Customer.Event
{
    public record AddressChangedEvent(Guid Id, Address OldAddress, Address NewAddress) : DomainEvent { }
}
