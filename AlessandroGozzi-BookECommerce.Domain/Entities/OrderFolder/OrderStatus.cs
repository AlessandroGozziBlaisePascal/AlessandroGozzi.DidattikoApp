using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder
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
