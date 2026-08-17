using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.ShipmentGestion.MarkShipmentAsDelivered
{
    public record MarkShipmentAsDeliveredCommand(
        Guid ShipmentId,
        Guid CustomerId
        ): IRequest<Result>;
}
