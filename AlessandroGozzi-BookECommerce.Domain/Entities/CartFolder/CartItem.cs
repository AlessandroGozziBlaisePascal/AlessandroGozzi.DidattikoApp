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
        public int Quantity { get; private set; }

        public CartItem(Guid bookId, int quantity)
        {
            Id = Guid.NewGuid();
            BookId = bookId;
            Quantity = quantity;
        }
        private CartItem() { }
        internal void UpdateQuantity(int quantity) => Quantity = quantity;
    }
}
