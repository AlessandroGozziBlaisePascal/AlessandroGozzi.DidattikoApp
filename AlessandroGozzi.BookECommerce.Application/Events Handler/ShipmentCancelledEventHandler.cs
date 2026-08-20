using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler.ShipmentCancelled
{
    public class ShipmentCancelledEventHandler: INotificationHandler<ShipmentCancelledEvent>
    {
        private readonly IShipmentRepository ShipRepo;
        private readonly ICustomerRepository CustRepo;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IOrderRepository OrderRepo;
        private readonly IEmailSender EmailSender;

        public ShipmentCancelledEventHandler(IShipmentRepository shipRepo, IEmailSender emailServer,IUnitOfWork unitOfWork, ICustomerRepository custRepo, IOrderRepository ordRepo)
        {
            ShipRepo = shipRepo;
            UnitOfWork = unitOfWork;
            CustRepo = custRepo;
            OrderRepo = ordRepo;
            EmailSender = emailServer;
        }

        public async Task Handle(ShipmentCancelledEvent notification, CancellationToken cancellationToken)
        {
            var shipment = await ShipRepo.GetByIdAsync(notification.Id, cancellationToken);
            if (shipment == null)
            {
                throw new Exception($"Shipment with ID {notification.Id} not found.");
            }

            var seller = await CustRepo.GetByIdAsync(notification.SellerId, cancellationToken);
            if(seller == null)
            {
                throw new Exception($"Seller with ID {notification.SellerId} not found.");
            }

            seller.Wallet.CancelPendingFunds(shipment.SubTotal);

            var order = await OrderRepo.GetByIdAsync(notification.OrderId, cancellationToken);
            if(order == null)
            {
                throw new Exception($"Order with ID {notification.OrderId} not found.");
            }   

            var buyer = await CustRepo.GetByIdAsync(order.CustomerId, cancellationToken);
            if(buyer == null)
            {
                throw new Exception($"Buyer with ID {order.CustomerId} not found.");
            }
            
            buyer.Wallet.Deposit(shipment.SubTotal);

            await EmailSender.SendEmailAsync(buyer.Email.ToDto(), "Shipment cancelled", $"You shipment got cancelled, refund got deposited in your wallet at {notification.OccurredOnUtc}", cancellationToken);
            await EmailSender.SendEmailAsync(seller.Email.ToDto(), "Shipment cancelled", "Your shipment got cancelled, pending funds got cancelled", cancellationToken);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
