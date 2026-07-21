using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder
{
    public class OrderItem
    {
        public Guid Id { get; init; }
        public Guid BookId { get; init; }
        public string BookTitle { get; private set; }
        public Money Price { get; init; }

        private OrderItem() { }

        private OrderItem(Guid id, Guid bookId,  string bookTitle, Money price)
        {
            Id = id;
            BookId = bookId;
            BookTitle = bookTitle;
            Price = price;
        }

        public static Result<OrderItem> Create(Guid id, Guid bookId, string bookTitle, Money price)
        {
            if (string.IsNullOrWhiteSpace(bookTitle))
                return Result.Failure<OrderItem>(new Error("BookTitle", "Title cannot be null", ErrorType.Validation));
            if (price == null)
                return Result.Failure<OrderItem>(new Error("BookPrice", "Price cannot be null", ErrorType.Validation));
            
            return Result.Success(new OrderItem(id, bookId, bookTitle, price));
        }
    }
}
