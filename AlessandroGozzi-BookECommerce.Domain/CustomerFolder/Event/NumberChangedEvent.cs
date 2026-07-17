using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Event
{
    public class NumberChangedEvent: DomainEvent
    {
        public Guid CustomerId { get; }
        public PhoneNumber OldNumber {  get; }
        public PhoneNumber NewNumber { get; }

        public NumberChangedEvent(Guid customerId, PhoneNumber oldNumber, PhoneNumber newNumber)
        {
            CustomerId = customerId;
            OldNumber = oldNumber;
            NewNumber = newNumber;
        }
    }
}
