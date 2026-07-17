using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Event
{
    public class EmailChangedEvent: DomainEvent
    {
        public Guid CustomerId { get; }
        public Email OldMail { get; }
        public Email NewMail { get; }

        public EmailChangedEvent(Guid customerId, Email oldMail, Email newMail)
        {
            CustomerId = customerId;
            OldMail = oldMail;
            NewMail = newMail;
        }
    }
}
