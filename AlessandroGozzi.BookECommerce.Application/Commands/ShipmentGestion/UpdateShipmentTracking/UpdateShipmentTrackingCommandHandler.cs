using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.ShipmentGestion.UpdateShipmentTracking
{
    public class UpdateShipmentTrackingCommandHandler: IRequestHandler<UpdateShipmentTrackingCommand, Result>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateShipmentTrackingCommandHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork)
        {
            _shipmentRepository = shipmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateShipmentTrackingCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);
            if (shipment == null)
                return Result.Failure(new Error("Shipment", "Shipment not found",ErrorType.NotFound));

            var updateResult = shipment.UpdateTracking(request.TrackingNumber, request.Carrier, request.CustomerId);
            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Error);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
