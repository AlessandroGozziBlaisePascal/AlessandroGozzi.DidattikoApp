using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.CustomerFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain
{
    public class Cart:Entity
    {
        private readonly Dictionary<Book, int> Books;
        public IReadOnlyDictionary<Book, int> GetBooks => Books;
        public Money ProvisionalCost => RefreshCost();

        public Cart()
        {
            Id = Guid.NewGuid();
            Books = new Dictionary<Book, int>();
        }

        public Money RefreshCost()
        {
            Money totalCost = Money.Create(0);
            foreach (var b in Books)
            {
                totalCost += b.Key.Price * b.Value;
            }
            return totalCost;
        }

        public Result AddBook(Book book, int quantity)
        {
            if (quantity <= 0)
            {
                return Result.Failure(new Error("Book", "Quantity must be greater than zero.", ErrorType.StatusConflict));
            }
            if (Books.ContainsKey(book))
            {
                Books[book] += quantity;
            }
            else
            {
                Books[book] = quantity;
            }
            return Result.Success();
        }

        public Result RemoveBook(Book book, int quantity)
        {
            if (quantity <= 0)
            {
                return Result.Failure(new Error("Book", "Quantity must be greater than zero.", ErrorType.StatusConflict));
            }
            if (!Books.ContainsKey(book))
            {
                return Result.Failure(new Error("Book", "Product not found in the shopping cart.", ErrorType.NotFound));
            }
            if (Books[book] < quantity)
            {
                return Result.Failure(new Error("Book", "Not enough quantity to remove.", ErrorType.StatusConflict));
            }
            Books[book] -= quantity;
            if (Books[book] == 0)
            {
                Books.Remove(book);
            }
            return Result.Success();
        }

        public Result ClearCart()
        {
            Books.Clear();
            return Result.Success();
        }
    }
}
}
