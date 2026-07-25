using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public CartItem(Guid bookId, string title, Money price, string mainPhoto, int quantity)
        {
            Id = Guid.NewGuid();
            BookId = bookId;
            BookTitle = title;
            Price = price;
            MainPhoto = mainPhoto;
            Quantity = quantity;
        }
        private CartItem() { }
        internal void UpdateQuantity(int quantity) => Quantity = quantity;
    }
}
