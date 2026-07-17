using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Event
{
    public class CustomerCreatedEvent: DomainEvent
    {
        public Guid CustomerId { get; }
        
        public CustomerCreatedEvent(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
