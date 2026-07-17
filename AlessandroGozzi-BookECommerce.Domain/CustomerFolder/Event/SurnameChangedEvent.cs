using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Event
{
    public class SurnameChangedEvent: DomainEvent
    {
        public Guid CustomerId { get; }
        public Surname OldSurname { get; }
        public Surname NewSurname { get; }

        public SurnameChangedEvent(Guid customerId, Surname oldSurname, Surname newSurname)
        {
            CustomerId = customerId;
            OldSurname = oldSurname;
            NewSurname = newSurname;
        }
    }
}
