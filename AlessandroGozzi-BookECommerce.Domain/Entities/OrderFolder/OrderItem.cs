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
        public Guid SellerId { get; init; }
        public string BookTitle { get; private set; }
        public Money Price { get; init; }
        public int Quantity { get; private set; }

        private OrderItem() { }

        private OrderItem(Guid bookId, Guid sellerId, string bookTitle, Money price, int quantity)
        {
            Id = Guid.NewGuid();
            BookId = bookId;
            SellerId = sellerId;    
            BookTitle = bookTitle;
            Price = price;
            Quantity = quantity;
        }

        public static Result<OrderItem> Create(Guid bookId, Guid sellerId, string bookTitle, Money price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(bookTitle))
                return Result.Failure<OrderItem>(new Error("BookTitle", "Title cannot be null", ErrorType.Validation));
            if (price == null)
                return Result.Failure<OrderItem>(new Error("BookPrice", "Price cannot be null", ErrorType.Validation));
            if(quantity < 1)
                return Result.Failure<OrderItem>(new Error("Quantity", "Quantity must be at min 1", ErrorType.Validation));
            return Result.Success(new OrderItem(bookId, sellerId, bookTitle, price, quantity));
        }
    }
}
