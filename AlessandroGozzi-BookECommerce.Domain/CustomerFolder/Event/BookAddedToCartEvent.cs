using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Event
{
    public class BookAddedToCartEvent: DomainEvent
    {
        public Guid CustomerId { get; }
        public Guid BookId { get; }
        public BookName BookName { get; }
        public Money BookPrice { get; }

        public BookAddedToCartEvent(Guid customerId, Guid bookId, BookName name, Money price)
        {
            CustomerId = customerId;
            BookId = bookId;
            BookName = name;
            BookPrice = price;
        }
    }
}
