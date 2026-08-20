using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler.TransitionsEvents
{
    public class WalletPendingFundsReleaseEventHandler: INotificationHandler<ShipmentDeliveredEvent>
    {
        private readonly ICustomerRepository CustomerRepo;
        private readonly IShipmentRepository ShipmentRepo;
        private readonly IEmailSender EmailSender;
        private readonly IUnitOfWork UnitOfWork;

        public WalletPendingFundsReleaseEventHandler(ICustomerRepository customerRepo, IShipmentRepository shipRepo, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            CustomerRepo = customerRepo;
            ShipmentRepo = shipRepo;
            EmailSender = emailSender;
            UnitOfWork = unitOfWork;
        }

        public async Task Handle(ShipmentDeliveredEvent shipmentEvent, CancellationToken token)
        {
            var shipment = await ShipmentRepo.GetByIdAsync(shipmentEvent.Id, token);
            if (shipment == null) return;

            var seller = await CustomerRepo.GetByIdAsync(shipmentEvent.SellerId, token);
            if (seller == null) return;

            var result = seller.Wallet.ReleasePendingFunds(shipment.SubTotal);
            if (result == null) return;

            await EmailSender.SendEmailAsync(seller.Email.ToDto(), "Pending Funds Release", $"You shipment got delivered at {shipmentEvent.OccurredOnUtc}, your pending funds({shipment.SubTotal} are now available in your wallet)", token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
}
