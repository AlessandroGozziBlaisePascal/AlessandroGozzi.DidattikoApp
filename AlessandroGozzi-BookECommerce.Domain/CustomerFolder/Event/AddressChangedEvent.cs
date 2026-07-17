using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Event
{
    public class AddressChangedEvent: DomainEvent
    {
        public Guid CustomerId { get; }
        public Address OldAddress { get; }
        public Address NewAddress { get; }  

        public AddressChangedEvent(Guid id, Address oldAddress, Address newAddress)
        {
            CustomerId = id;
            OldAddress = oldAddress;
            NewAddress = newAddress;
        }
    }
}
