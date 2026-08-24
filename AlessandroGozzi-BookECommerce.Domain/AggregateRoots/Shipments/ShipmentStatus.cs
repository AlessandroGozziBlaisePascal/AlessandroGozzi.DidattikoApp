using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments
{
    public enum ShipmentStatus
    {
        Preparing,
        Shipped,
        Delivered,
        Cancelled
    }
}
