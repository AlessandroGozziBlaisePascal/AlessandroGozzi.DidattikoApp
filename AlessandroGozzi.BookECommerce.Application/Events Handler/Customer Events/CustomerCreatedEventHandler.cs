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
    public class CustomerCreatedEventHandler: INotificationHandler<CustomerCreatedEvent>
    {
        private readonly ICustomerRepository CustomerRepository;
        private readonly IEmailSender EmailSender;
        private readonly IUnitOfWork UnitOfWork;

        public CustomerCreatedEventHandler(ICustomerRepository customerRepository, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            CustomerRepository = customerRepository;
            EmailSender = emailSender;
            UnitOfWork = unitOfWork;
        }

        public async Task Handle(CustomerCreatedEvent notification, CancellationToken token)
        {
            var customer = await CustomerRepository.GetByIdAsync(notification.CustomerId, token);
            if (customer == null) return;

            await EmailSender.SendEmailAsync(customer.Email.ToDto(), "Welcome Email", $"Welcome {customer.FullName} to BookECommerce", token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
} 
