using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Event
{
    public class NameChangedEvent: DomainEvent
    {
        public Guid CustomerId { get; }
        public Name OldName { get; }
        public Name NewName { get; }

        public NameChangedEvent(Guid customerId, Name oldName, Name newName)
        {
            CustomerId = customerId;
            OldName = oldName;
            NewName = newName;
        }
    }
}
