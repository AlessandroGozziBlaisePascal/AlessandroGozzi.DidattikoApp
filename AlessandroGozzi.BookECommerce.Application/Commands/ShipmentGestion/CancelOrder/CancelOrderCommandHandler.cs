using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.ShipmentGestion.CancelOrder
{
    public class CancelOrderCommandHandler: IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IOrderRepository OrderRepo;
        private readonly IShipmentRepository ShipmentRepo;
        private readonly IUnitOfWork UnitOfWork;

        public CancelOrderCommandHandler(IOrderRepository orderRepo, IShipmentRepository shipRepo, IUnitOfWork unitOfWork)
        {
            OrderRepo = orderRepo;
            ShipmentRepo = shipRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CancelOrderCommand command, CancellationToken token)
        {
            var order = await OrderRepo.GetByIdAsync(command.OrderId, token);

            if(order == null)
            {
                return Result.Failure(new Error("Order", "Order not found", ErrorType.NotFound));
            }
            if(order.CustomerId != command.CustomerId)
            {
                return Result.Failure(new Error("Order", "Order customer is not the same", ErrorType.PermissionDenied));
            }

            var shipments = await ShipmentRepo.GetByIdsAsync(order.ShipmentIds.ToList(), token);
            var alreadyShipped = shipments.Any(s => s.Status == ShipmentStatus.Shipped);
            if(alreadyShipped)
            {
                return Result.Failure(new Error("Order", "Cannot cancel the order because it has already been shipped", ErrorType.StatusConflict));
            }

            foreach(var shipment in shipments)
            {
                var result = shipment.CancelShipment(command.CustomerId);
                if(result.IsFailure)
                {
                    return Result.Failure(new Error("Shipment", "Cannot cancel the shipment", ErrorType.Failure));
                }
            }

            var actionresult = order.CancelOrder();
            if(actionresult.IsFailure)
                return Result.Failure(new Error("Order", "Cannot cancel the order", ErrorType.Failure));

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
