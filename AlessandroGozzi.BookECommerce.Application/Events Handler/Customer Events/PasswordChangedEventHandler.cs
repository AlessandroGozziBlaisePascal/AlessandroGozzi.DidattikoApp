using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.Events;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler
{
    public class PasswordChangedEventHandler: INotificationHandler<CustomerPasswordChangedEvent>
    {
        private readonly ICustomerRepository CustomerRepository;
        private readonly IEmailSender EmailSender;
        private readonly IUnitOfWork UnitOfWork;

        public PasswordChangedEventHandler(ICustomerRepository customerRepository, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            CustomerRepository = customerRepository;
            EmailSender = emailSender;
            UnitOfWork = unitOfWork;
        }

        public async Task Handle(CustomerPasswordChangedEvent customerEvent, CancellationToken token)
        {
            var customer = await CustomerRepository.GetByIdAsync(customerEvent.CustomerId, token);
            if (customer == null) return;

            await EmailSender.SendEmailAsync(customer.Email.Value, "Personal Password",
                $"Your personal password got changed at {customerEvent.OccurredOnUtc}",token);
        }
    }
}
