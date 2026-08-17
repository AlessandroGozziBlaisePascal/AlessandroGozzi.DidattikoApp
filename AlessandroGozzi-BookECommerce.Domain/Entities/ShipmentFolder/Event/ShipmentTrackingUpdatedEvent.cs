using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Event
{
    public record ShipmentTrackingUpdatedEvent (Guid Id, Guid OrderId, Guid SellerId, TrackingInfo TrackInfo): DomainEvent;
}
