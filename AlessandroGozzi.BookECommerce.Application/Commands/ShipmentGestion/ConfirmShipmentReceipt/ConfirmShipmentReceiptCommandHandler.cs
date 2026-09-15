using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.ShipmentGestion.ConfirmShipmentReceipt
{
    public class ConfirmShipmentReceiptCommandHandler: IRequestHandler<ConfirmShipmentReceiptCommand, Result>
    {
        private readonly IShipmentRepository ShipmentRepository;
        private readonly IUnitOfWork UnitOfWork;

        public ConfirmShipmentReceiptCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
        {
            ShipmentRepository = shipmentRepository;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ConfirmShipmentReceiptCommand command, CancellationToken token)
        {
            var shipment = await ShipmentRepository.GetByIdAsync(command.ShipmentId, token);
            if (shipment == null)
            {
                return Result.Failure(new Error("Shipment", "Shipment not found", ErrorType.NotFound));
            }

            var result = shipment.MarkAsDelivered(command.BuyerId);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
