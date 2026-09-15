using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.ValueObjects;

namespace AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Orders
{
    public class OrderItem:Entity
    {
        public Guid BookId { get; init; }
        public Guid SellerId { get; init; }
        public string BookTitle { get; private set; }
        public Money Price { get; init; }
        public ImageUrl MainPhoto { get; init; }
        public int Quantity { get; private set; }

        public Money TotalPrice => Price * Quantity;

        private OrderItem() { }

        private OrderItem(Guid bookId, Guid sellerId, string bookTitle, Money price, ImageUrl mainPhoto, int quantity)
        {
            BookId = bookId;
            SellerId = sellerId;
            BookTitle = bookTitle;
            Price = price;
            Quantity = quantity;
            MainPhoto = mainPhoto;

        }

        public static Result<OrderItem> Create(Guid bookId, Guid sellerId, string bookTitle, Money price, ImageUrl mainPhoto, int quantity)
        {
            if (bookId == Guid.Empty)
                return Result.Failure<OrderItem>(new Error("BookId", "Book ID cannot be empty", ErrorType.Validation));

            if (sellerId == Guid.Empty)
                return Result.Failure<OrderItem>(new Error("SellerId", "Seller ID cannot be empty", ErrorType.Validation));

            if (string.IsNullOrWhiteSpace(bookTitle))
                return Result.Failure<OrderItem>(new Error("BookTitle", "Title cannot be null or whitespace", ErrorType.Validation));

            if (price is null)
                return Result.Failure<OrderItem>(new Error("BookPrice", "Price cannot be null", ErrorType.Validation));

            if (quantity < 1)
                return Result.Failure<OrderItem>(new Error("Quantity", "Quantity must be at least 1", ErrorType.Validation));

            if (mainPhoto is null)
                return Result.Failure<OrderItem>(new Error("MainPhoto", "Book photo cannot be null", ErrorType.Validation));

            return Result.Success(new OrderItem(bookId, sellerId, bookTitle, price, mainPhoto, quantity));
        }
    }
}
