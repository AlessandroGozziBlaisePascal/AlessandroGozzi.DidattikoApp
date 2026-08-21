using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.ShipmentGestion.MarkShipmentAsShipped
{
    public record MarkShipmentAsShippedCommand(
        Guid ShipmentId,
        string? TrackingNumber,
        string? Carrier,
        Guid SellerId
        ): IRequest<Result>;
}
