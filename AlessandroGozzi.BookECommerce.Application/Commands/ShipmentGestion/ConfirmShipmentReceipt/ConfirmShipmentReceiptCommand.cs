using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.ShipmentGestion.ConfirmShipmentReceipt
{
    public record ConfirmShipmentReceiptCommand(
        Guid ShipmentId,
        Guid BuyerId
        ): IRequest<Result>;
}
