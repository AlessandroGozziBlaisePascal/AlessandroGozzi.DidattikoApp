using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments.ValueObjects;

namespace AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments.Events
{
    public record ShipmentShippedEvent (Guid Id, Guid OrderId, Guid SellerId, TrackingInfo TrackInfo): DomainEvent;
}
