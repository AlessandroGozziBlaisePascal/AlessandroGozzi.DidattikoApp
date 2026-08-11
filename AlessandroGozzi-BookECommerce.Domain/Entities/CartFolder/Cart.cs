using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Event;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder
{
    public class Cart:Entity
    {
        private readonly List<CartItem> Items = new ();
        public IReadOnlyCollection<CartItem> GetItems => Items;
        public Guid CustomerId { get; init; }

        private Cart() { }
        private Cart(Guid customerId) => CustomerId = customerId; 

        public static Result<Cart> Create(Guid customerId)
        {
            if(customerId == Guid.Empty)
            {
                return Result.Failure<Cart>(new Error("Customer id", "Id is empty", ErrorType.Validation));
            }
            return Result.Success(new Cart(customerId));
        }

        public Result AddItem(Guid bookId, Guid sellerId, string bookTitle, Money price, string mainPhoto, int quantity)
        {
            if (quantity <= 0)
            {
                return Result.Failure(new Error("Book", "Quantity must be greater than zero.", ErrorType.StatusConflict));
            }
            var item = Items.FirstOrDefault(i  => i.BookId == bookId);
            if (item != null)
                item.UpdateQuantity(item.Quantity + quantity);
            else
            {
                var result = CartItem.Create(bookId, sellerId, bookTitle, price, mainPhoto, quantity);
                if (result.IsFailure)
                    return Result.Failure(new Error("Book", "Book failed to be created", ErrorType.Failure));
                Items.Add(result.Value);
            }
                     
            Raise(new BookAddedToCartEvent(CustomerId, Id, bookId, quantity));

            return Result.Success();
        }

        public Result RemoveItem(Guid bookId, int quantity)
        {
            if (quantity <= 0)
            {
                return Result.Failure(new Error("Book", "Quantity must be greater than zero.", ErrorType.StatusConflict));
            }
            var item = Items.FirstOrDefault(i => i.BookId == bookId);
            if(item == null)
                return Result.Failure(new Error("Book", "Product not found in the shopping cart.", ErrorType.NotFound));
            if(item.Quantity < quantity)
                return Result.Failure(new Error("Book", "Not enough quantity to remove.", ErrorType.StatusConflict));
            if(item.Quantity == quantity)
                Items.Remove(item);
            else
                item.UpdateQuantity(item.Quantity - quantity);
            Raise(new BookRemovedFromCartEvent(CustomerId, Id, bookId, quantity));
            return Result.Success();
        }

        public Result ClearCart()
        {
            Items.Clear();
            return Result.Success();
        }
    }

}
