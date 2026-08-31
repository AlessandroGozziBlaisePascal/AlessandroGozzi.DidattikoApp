using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Buyer_POV.TopUpWallet.Action
{
    public class TopUpWalletActionCommandHandler: IRequestHandler<TopUpWalletActionCommand, Result>
    {
        private readonly IPaymentService _paymentService;
        private readonly ICustomerRepository CustomerRepository;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IWalletRepository WalletRepo;

        public TopUpWalletActionCommandHandler(IPaymentService paymentService, ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IWalletRepository walletRepo)
        {
            _paymentService = paymentService;
            CustomerRepository = customerRepository;
            UnitOfWork = unitOfWork;
            WalletRepo = walletRepo;
        }

        public async Task<Result> Handle(TopUpWalletActionCommand command, CancellationToken token)
        {
            var paymentDetailsResult = await _paymentService.IsPaymentSuccessfulAsync(command.PaymentIntentId, token);
            if (paymentDetailsResult.IsFailure)
                return Result.Failure(paymentDetailsResult.Error);

            var customer = await CustomerRepository.GetByIdAsync(command.CustomerId, token);
            if (customer == null)
                return Result.Failure(new Error("Customer", "Customer not found", ErrorType.NotFound));

            var conversionResult = command.Amount.ToMoneyDomain();
            if(conversionResult.IsFailure)
                return Result.Failure(conversionResult.Error);

            var wallet = await WalletRepo.GetByCustomerId(command.CustomerId, token);
            if (wallet == null)
                return Result.Failure(new Error("Wallet", "Wallet not found", ErrorType.NotFound));

            var result = wallet.Deposit(conversionResult.Value);
            if(result.IsFailure)
                return Result.Failure(result.Error);

            await UnitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
