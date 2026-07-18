using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain
{
    public class Cart:Entity
    {
        private readonly List<CartItem> Items;
        public IReadOnlyCollection<CartItem> GetItems => Items;

        public Cart()
        {
            Id = Guid.NewGuid();
            Items = new List<CartItem>();
        }

        public Result AddBook(Book book, int quantity)
        {
            if (quantity <= 0)
            {
                return Result.Failure(new Error("Book", "Quantity must be greater than zero.", ErrorType.StatusConflict));
            }var item = 
            if (Items.Contains(book))
            {
                Items[book] += quantity;
            }
            else
            {
                Items[book] = quantity;
            }
            return Result.Success();
        }

        public Result RemoveBook(Book book, int quantity)
        {
            if (quantity <= 0)
            {
                return Result.Failure(new Error("Book", "Quantity must be greater than zero.", ErrorType.StatusConflict));
            }
            if (!Items.ContainsKey(book))
            {
                return Result.Failure(new Error("Book", "Product not found in the shopping cart.", ErrorType.NotFound));
            }
            if (Items[book] < quantity)
            {
                return Result.Failure(new Error("Book", "Not enough quantity to remove.", ErrorType.StatusConflict));
            }
            Items[book] -= quantity;
            if (Items[book] == 0)
            {
                Items.Remove(book);
            }
            return Result.Success();
        }

        public Result ClearCart()
        {
            Items.Clear();
            return Result.Success();
        }
    }

    public class CartItem
    {
        public Guid Id { get; private set; }
        public Guid BookId { get; private set; }
        public int Quantity { get; private set; }

        internal CartItem( Guid bookId, int quantity)
        {
            Id = Guid.NewGuid();
            BookId = bookId;
            Quantity = quantity;
        }
        private CartItem() { }
        internal void UpdateQuantity( int quantity ) => Quantity = quantity;

    }
}
