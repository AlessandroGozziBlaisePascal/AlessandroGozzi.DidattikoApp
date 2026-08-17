using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.ShipmentGestion.UpdateShipmentTracking
{
    public record UpdateShipmentTrackingCommand(
        Guid ShipmentId,
        string TrackingNumber,
        string Carrier,
        Guid CustomerId
        ): IRequest<Result>;
}
