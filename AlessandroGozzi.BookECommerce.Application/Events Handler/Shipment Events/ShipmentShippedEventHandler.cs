using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Shipments.Events;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler
{
    public class ShipmentShippedEventHandler: INotificationHandler<ShipmentShippedEvent>
    {
        private readonly IShipmentRepository ShipmentRepo;
        private readonly ICustomerRepository CustomerRepo;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IEmailSender EmailSender;

        public ShipmentShippedEventHandler(IShipmentRepository shipRepo, IEmailSender emailServer, IUnitOfWork unitOfWork, ICustomerRepository custRepo)
        {
            ShipmentRepo = shipRepo;
            UnitOfWork = unitOfWork;
            CustomerRepo = custRepo;
            EmailSender = emailServer;
        }

        public async Task Handle(ShipmentShippedEvent notification, CancellationToken token)
        {
            var shipment = await ShipmentRepo.GetByIdAsync(notification.Id, token);
            if (shipment == null) return;

            var buyer = await CustomerRepo.GetByIdAsync(notification.SellerId, token);
            if (buyer == null) return;
            var seller = await CustomerRepo.GetByIdAsync(notification.SellerId, token);
            if (seller == null) return;

            await EmailSender.SendEmailAsync(buyer.Email.Value, "Shipment", 
                $"your order has been entrusted to {notification.TrackInfo.Carrier} with tracking code: " +
                $"{notification.TrackInfo.TrackingCode} at {notification.OccurredOnUtc}. " +
                $"Check Url if exist: {notification.TrackInfo.TrackingUrl}", token);

            await EmailSender.SendEmailAsync(seller.Email.Value, "Shipment shipped",$"Your shipment got entrusted with success at {notification.OccurredOnUtc}, " +
                $"pendings funds ({shipment.SubTotal} got deposited in your wallet)",token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
}
