using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.CustomerEvent;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler
{
    public class CreditCardRemovedEventHandler: INotificationHandler<CreditCardRemovedEvent>
    {
        private readonly IEmailSender EmailSender;
        private readonly ICustomerRepository CustomerRepo;
        private readonly IUnitOfWork UnitOfWork;

        public CreditCardRemovedEventHandler(IEmailSender emailSender, IUnitOfWork unitOfWork, ICustomerRepository custRepo)
        {
            EmailSender = emailSender;
            UnitOfWork = unitOfWork;
            CustomerRepo = custRepo;
        }

        public async Task Handle(CreditCardRemovedEvent notification, CancellationToken token)
        { 
            var customer = await CustomerRepo.GetByIdAsync(notification.CustomerId, token);
            if (customer == null) return;

            await EmailSender.SendEmailAsync(customer.Email.ToDto(),"Credit card removed",
                $"Your credit card {notification.CardOwner} {notification.DisplayName} got removed with success at {notification.OccurredOnUtc}", token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
}
