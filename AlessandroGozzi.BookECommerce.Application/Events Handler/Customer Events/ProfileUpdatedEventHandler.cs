using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Customers.Events;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler
{
    public class ProfileUpdatedEventHandler: INotificationHandler<ProfileUpdatedEvent>
    {
        private readonly IEmailSender EmailSender;
        private readonly ICustomerRepository CustomerRepository;
        private readonly IUnitOfWork UnitOfWork;

        public ProfileUpdatedEventHandler(IEmailSender emailSender, ICustomerRepository customerRepository, IUnitOfWork uOfWork)
        {
            EmailSender = emailSender;
            CustomerRepository = customerRepository;
            UnitOfWork = uOfWork;
        }

        public async Task Handle(ProfileUpdatedEvent notification, CancellationToken token)
        {
            var customer = await CustomerRepository.GetByIdAsync(notification.CustomerId, token);
            if (customer == null) return;

            string fields = string.Join(", ", notification.UpdatedFields);
            string body = $"{customer.FullName}, profile fields: {fields} got updated at {notification.OccurredOnUtc}";

            await EmailSender.SendEmailAsync(customer.Email.Value, "Profile Updated",body, token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
}
