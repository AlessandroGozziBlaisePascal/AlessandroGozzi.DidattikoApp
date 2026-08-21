using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler
{
    public class ShipmentDeliveredEventHandler: INotificationHandler<ShipmentDeliveredEvent>
    {
        private readonly IShipmentRepository ShipmentRepo;
        private readonly ICustomerRepository CustomerRepo;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IEmailSender EmailSender;

        public ShipmentDeliveredEventHandler(IShipmentRepository shipRepo, IEmailSender emailServer, IUnitOfWork unitOfWork, ICustomerRepository custRepo)
        {
            ShipmentRepo = shipRepo;
            UnitOfWork = unitOfWork;
            CustomerRepo = custRepo;
            EmailSender = emailServer;
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

            var result = seller.Wallet.ReleasePendingFunds(shipment.SubTotal);
            if (result.IsFailure)
            {
                throw new Exception($"{result.Error.Description}");
            }

            await EmailSender.SendEmailAsync(seller.Email.ToDto(), "Pending Funds Release", $"You shipment got " +
                $"delivered at {notification.OccurredOnUtc}, " +
                $"your pending funds({shipment.SubTotal}) are now available in your wallet", token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
}
