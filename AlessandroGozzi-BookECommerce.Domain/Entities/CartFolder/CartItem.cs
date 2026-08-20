using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder
{
    public class CartItem
    {
        public Guid BookId { get; private set; }
        public Guid SellerId { get; private set; }
        public string BookTitle { get; private set; }
        public Money Price { get; init; }
        public string MainPhoto { get; init; }
        public int Quantity { get; private set; }

        private CartItem(Guid bookId, Guid sellerId, string title, Money price, string mainPhoto)
        {
            BookId = bookId;
            BookTitle = title;
            Price = price;
            MainPhoto = mainPhoto;
            Quantity = 1;
            SellerId = sellerId;
        }
        private CartItem() { }
        public Result UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                return Result.Failure(new Error("Quantity", "Quantity must be greater than 1", ErrorType.Validation));
            Quantity = quantity;
            return Result.Success();
        }

        public static Result<CartItem> Create(Guid bookId, Guid sellerId, string title, Money price, string mainPhoto)
        {
            if (bookId == Guid.Empty)
                return Result.Failure<CartItem>(new Error("Book id", "Book id is empty", ErrorType.Validation));
            if (sellerId == Guid.Empty)
                return Result.Failure<CartItem>(new Error("Seller id", "Seller id is empty", ErrorType.Validation));
            if (string.IsNullOrWhiteSpace(title))
                return Result.Failure<CartItem>(new Error("Book title", "Book title is empty", ErrorType.Validation));
            if (price == null || price.Amount <= 0)
                return Result.Failure<CartItem>(new Error("Price", "Price is invalid", ErrorType.Validation));
            if (string.IsNullOrWhiteSpace(mainPhoto))
                return Result.Failure<CartItem>(new Error("Main photo", "Main photo is empty", ErrorType.Validation));
            var cartItem = new CartItem(bookId, sellerId, title, price, mainPhoto);
            return Result.Success(cartItem);
        }
    }
}
