using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.ShipmentGestion.MarkShipmentAsDelivered
{
    public class MarkShipmentAsDeliveredCommandHandler: IRequestHandler<MarkShipmentAsDeliveredCommand, Result>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public MarkShipmentAsDeliveredCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
        {
            _shipmentRepository = shipmentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(MarkShipmentAsDeliveredCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);
            if (shipment == null)
                return Result.Failure(new Error("Shipment", "Shipment not found",ErrorType.NotFound));

            var markAsDeliveredResult = shipment.MarkAsDelivered(request.CustomerId);
            if (markAsDeliveredResult.IsFailure)
                return Result.Failure(markAsDeliveredResult.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
