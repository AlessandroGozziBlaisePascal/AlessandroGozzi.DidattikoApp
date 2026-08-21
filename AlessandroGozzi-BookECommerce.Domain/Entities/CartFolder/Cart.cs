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

        public Result AddItem(Guid bookId, Guid sellerId, string bookTitle, Money price, string mainPhoto)
        {
            var item = Items.FirstOrDefault(i  => i.BookId == bookId);
            if (item != null)
            {
                var result = item.UpdateQuantity(item.Quantity + 1);
                if (result.IsFailure)
                    return Result.Failure(result.Error);
            }   
            else
            {
                var result = CartItem.Create(bookId, sellerId, bookTitle, price, mainPhoto);
                if (result.IsFailure)
                    return Result.Failure(new Error("Book", "Book failed to be created", ErrorType.Failure));
                Items.Add(result.Value);
            }
                     
            Raise(new BookAddedToCartEvent(CustomerId, Id, bookId));

            return Result.Success();
        }

        public Result RemoveItem(Guid bookId)
        {
            var item = Items.FirstOrDefault(i => i.BookId == bookId);
            if(item == null)
                return Result.Failure(new Error("Book", "Product not found in the shopping cart.", ErrorType.NotFound));
            Items.Remove(item);
            Raise(new BookRemovedFromCartEvent(CustomerId, Id, bookId));
            return Result.Success();
        }

        public Result UpdateItemQuantity(Guid bookId, int newQuantity)
        {
            var item = Items.FirstOrDefault(i => i.BookId == bookId);
            if (item == null)
                return Result.Failure(new Error("Book", "Product not found in the shopping cart.", ErrorType.NotFound));
            if(newQuantity == 0)
            {
                var remotionResult = RemoveItem(bookId);
                if (remotionResult.IsFailure)
                    return Result.Failure(remotionResult.Error);
            }

            var result = item.UpdateQuantity(newQuantity);
            if (result.IsFailure)
                return Result.Failure(result.Error);
            return Result.Success();
        }

        public Result ClearCart()
        {
            Items.Clear();
            return Result.Success();
        }
    }

}
