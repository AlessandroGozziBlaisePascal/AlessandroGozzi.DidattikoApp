using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments.Events;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Wallets;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler
{
    public class ShipmentDeliveredEventHandler: INotificationHandler<ShipmentDeliveredEvent>
    {
        private readonly IShipmentRepository ShipmentRepo;
        private readonly ICustomerRepository CustomerRepo;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IEmailSender EmailSender;
        private readonly IWalletRepository WalletRepo;

        public ShipmentDeliveredEventHandler(IShipmentRepository shipRepo, IEmailSender emailServer, IUnitOfWork unitOfWork, ICustomerRepository custRepo, IWalletRepository walletRepo)
        {
            ShipmentRepo = shipRepo;
            UnitOfWork = unitOfWork;
            CustomerRepo = custRepo;
            EmailSender = emailServer;
            WalletRepo = walletRepo;
        }

        public async Task Handle(ShipmentDeliveredEvent notification, CancellationToken token)
        {
            var shipment = await ShipmentRepo.GetByIdAsync(notification.Id, token);
            if (shipment == null)
            {
                throw new Exception($"Shipment with ID {notification.Id} not found.");
            }

            var seller = await CustomerRepo.GetByIdAsync(notification.SellerId, token);
            if (seller == null)
            {
                throw new Exception($"Seller with ID {notification.SellerId} not found.");
            }
            var wallet = await WalletRepo.GetByCustomerId(notification.SellerId, token);
            if (wallet == null) return;

            var result = wallet.ReleasePendingFunds(shipment.SubTotal);
            if (result.IsFailure)
            {
                throw new Exception($"{result.Error.Description}");
            }

            await EmailSender.SendEmailAsync(seller.Email.Value, "Pending Funds Release", $"You shipment got " +
                $"delivered at {notification.OccurredOnUtc}, " +
                $"your pending funds({shipment.SubTotal}) are now available in your wallet", token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
}
