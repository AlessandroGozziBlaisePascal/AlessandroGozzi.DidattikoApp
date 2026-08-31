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
    public class WalletWithdrawalRequestEventHandler: INotificationHandler<WithdrawalRequestedEvent>
    {
        private readonly IPayoutService _payoutService;
        private readonly ICustomerRepository CustomerRepository;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IEmailSender EmailSender;
        private readonly IWalletRepository WalletRepo;

        public WalletWithdrawalRequestEventHandler(IPayoutService payoutService, IWalletRepository walletRepo, ICustomerRepository custRepo, IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _payoutService = payoutService;
            CustomerRepository = custRepo;
            UnitOfWork = unitOfWork;
            EmailSender = emailSender;
            WalletRepo = walletRepo;
        }

        public async Task Handle(WithdrawalRequestedEvent request, CancellationToken cancellationToken)
        {
            var wallet = await WalletRepo.GetByCustomerId(request.CustomerId, cancellationToken);
            if (wallet == null)
            {
                throw new InvalidOperationException("Wallet not found");
            }

            var customer = await CustomerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null)
            {
                wallet.Deposit(request.Amount);
                await UnitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            var result = await _payoutService.SendPayoutAsync(request.CustomerId, request.Amount.Amount, request.Iban.Value, cancellationToken);
            if (result.IsFailure)
            {
                wallet.Deposit(request.Amount);
                await EmailSender.SendEmailAsync(customer!.Email.Value, "Wallet withdrawal error",
                    $"An error occurred during withdraw operation, balance redeposited in your wallet", cancellationToken);
                await UnitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }
               
            await EmailSender.SendEmailAsync(customer!.Email.Value, "Wallet withdrawal", $"Your amount got send {request.OccurredOnUtc})", cancellationToken);

            await UnitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
