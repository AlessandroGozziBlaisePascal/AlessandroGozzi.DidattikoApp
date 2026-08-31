using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments.Events;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
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
        private readonly IWalletRepository WalletRepo;

        public ShipmentCancelledEventHandler(IShipmentRepository shipRepo, IWalletRepository walletRepo, IEmailSender emailServer,IUnitOfWork unitOfWork, ICustomerRepository custRepo, IOrderRepository ordRepo)
        {
            ShipRepo = shipRepo;
            UnitOfWork = unitOfWork;
            CustRepo = custRepo;
            OrderRepo = ordRepo;
            EmailSender = emailServer;
            WalletRepo = walletRepo;
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
            var sellerWallet = await WalletRepo.GetByCustomerId(notification.SellerId, cancellationToken);
            if (sellerWallet == null)
            {
                throw new Exception($"Wallet not found.");
            }
            sellerWallet.CancelPendingFunds(shipment.SubTotal); 
            var buyer = await CustRepo.GetByIdAsync(shipment.BuyerId, cancellationToken);
            if(buyer == null)
            {
                throw new Exception($"Buyer with ID {shipment.BuyerId} not found.");
            }
            var buyerWallet = await WalletRepo.GetByCustomerId(buyer.Id, cancellationToken);
            if (buyerWallet == null)
            {
                throw new Exception($"Wallet not found.");
            }
            buyerWallet.Deposit(shipment.SubTotal);

            await EmailSender.SendEmailAsync(buyer.Email.Value, "Shipment cancelled", 
                $"You shipment got cancelled, refund got deposited in your wallet at {notification.OccurredOnUtc}", cancellationToken);

            await EmailSender.SendEmailAsync(seller.Email.Value, "Shipment cancelled",
                "Your shipment got cancelled, pending funds got cancelled", cancellationToken);

            await UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
