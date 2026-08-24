using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Wallets.Events;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Events_Handler.TransitionsEvents
{
    public class WalletWithdrawalRequestEventHandler: INotificationHandler<BalanceWithdrawedEvent>
    {
        private readonly IPayoutService _payoutService;
        private readonly ICustomerRepository CustomerRepository;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IEmailSender EmailSender;

        public WalletWithdrawalRequestEventHandler(IPayoutService payoutService, ICustomerRepository custRepo, IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _payoutService = payoutService;
            CustomerRepository = custRepo;
            UnitOfWork = unitOfWork;
            EmailSender = emailSender;
        }

        public async Task Handle(BalanceWithdrawedEvent request, CancellationToken cancellationToken)
        {
            var result = await _payoutService.SendPayoutAsync(request.CustomerId, request.Money.ToDto(), cancellationToken);
            if (result.IsFailure) return;

            var customer = await CustomerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null) return;

            customer.Wallet.Deposit(request.Money);

            await EmailSender.SendEmailAsync(customer.Email.ToDto(), "Wallet withdrawal", $"Your amount got deposited from your card({customer.CreditCard!.DisplayName} to your wallet balance at {request.OccurredOnUtc})", cancellationToken);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
