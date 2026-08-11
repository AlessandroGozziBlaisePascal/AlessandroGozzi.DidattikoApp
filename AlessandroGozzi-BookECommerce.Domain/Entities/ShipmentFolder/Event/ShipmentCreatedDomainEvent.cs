using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Event
{
    public record ShipmentCreatedDomainEvent(Guid Id, Guid OrderId, Guid SellerId): DomainEvent;
}
