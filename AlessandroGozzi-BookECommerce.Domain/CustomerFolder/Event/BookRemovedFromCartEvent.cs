using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Event
{
    public class BookRemovedFromCartEvent: DomainEvent
    {
        public Guid CustomerId { get; }
        public Guid BookId { get; }
        public BookName BooKName { get; }
        public Money Price { get; }

        public BookRemovedFromCartEvent(Guid customerId, Guid bookId, BookName booKName, Money price)
        {
            CustomerId = customerId;
            BookId = bookId;
            BooKName = booKName;
            Price = price;
        }
    }
}
