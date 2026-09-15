using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Wallets.Events;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler
{
    public class BalanceDepositedInWalletEventHandler: INotificationHandler<BalanceDepositedEvent>
    {
        private readonly ICustomerRepository CustomerRepo;
        private readonly IEmailSender EmailSender;
        private readonly IUnitOfWork UnitOfWork;

        public BalanceDepositedInWalletEventHandler(ICustomerRepository custRepo, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            CustomerRepo = custRepo;
            EmailSender = emailSender;
            UnitOfWork = unitOfWork;
        }

        public async Task Handle(BalanceDepositedEvent notification, CancellationToken token)
        {
            var customer = await CustomerRepo.GetByIdAsync(notification.CustomerId, token);
            if (customer == null) return;

            await EmailSender.SendEmailAsync(customer.Email.Value, "Wallet transitin", 
                $"Balance of {notification.Money.Amount} got deposited in your wallet at {notification.OccurredOnUtc}", token);

            await UnitOfWork.SaveChangesAsync(token);
        }
    }
}
