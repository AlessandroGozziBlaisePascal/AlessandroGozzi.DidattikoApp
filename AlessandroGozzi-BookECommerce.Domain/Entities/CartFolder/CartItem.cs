using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder
{
    public class CartItem
    {
        public Guid Id { get; private set; }
        public Guid BookId { get; private set; }
        public string BookTitle { get; private set; }
        public Money Price { get; init; }
        public string MainPhoto { get; init; }
        public int Quantity { get; private set; }

        private CartItem(Guid bookId, string title, Money price, string mainPhoto, int quantity)
        {
            Id = Guid.NewGuid();
            BookId = bookId;
            BookTitle = title;
            Price = price;
            MainPhoto = mainPhoto;
            Quantity = quantity;
        }
        private CartItem() { }

        public static Result<CartItem> Create(Guid bookId, string title, Money price, string mainPhoto, int quantity)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result.Failure<CartItem>(new Error("Cart item title", "Title is null", ErrorType.Validation));
            if(price == null)
                return Result.Failure<CartItem>(new Error("Cart item price", "Price is null", ErrorType.Validation));
            if(price.Amount < 0)
                return Result.Failure<CartItem>(new Error("Cart item price", "Price can't be negative", ErrorType.Validation));
            if(string.IsNullOrWhiteSpace(mainPhoto))
                return Result.Failure<CartItem>(new Error("Cart item main photo", "Photo url is null", ErrorType.Validation));
            if(quantity < 1)
                return Result.Failure<CartItem>(new Error("Cart item quantity", "Quantity must be at least 1", ErrorType.Validation));
            return Result.Success(new CartItem(bookId, title, price, mainPhoto, quantity));
        }
        public Result UpdateQuantity(int quantity)
        {
            if(quantity < 1)
                return Result.Failure(new Error("Update quantity", "Quantity must be at least 1", ErrorType.Validation));
            else
            {
                Quantity += quantity;
                return Result.Success();
            }
        }
    }
}
