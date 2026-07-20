using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.Order
{
    public enum OrderStatus
    {
        Placed,
        Confirmed,
        Prepared,
        Shipped,
        Delivered,
        Cancelled
    }
}
