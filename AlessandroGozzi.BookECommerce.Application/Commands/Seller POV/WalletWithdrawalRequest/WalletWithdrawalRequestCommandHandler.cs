using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Seller_POV.WalletWithdrawalRequest
{
    public class WalletWithdrawalRequestCommandHandler: IRequestHandler<WalletWithdrawalRequestCommand, Result>
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly IWalletRepository WalletRepo;

        public WalletWithdrawalRequestCommandHandler(IUnitOfWork unitOfWork, IWalletRepository walletRepo)
        {
            UnitOfWork = unitOfWork;
            WalletRepo = walletRepo;
        }

        public async Task<Result> Handle(WalletWithdrawalRequestCommand command, CancellationToken token)
        {
            if(command.Amount < 5m)
            {
                return Result.Failure(new Error("Withdrawal amount", "Min withdrawal amount is 5 €", ErrorType.Validation));
            }

            var wallet = await WalletRepo.GetByCustomerId(command.CustomerId, token);
            if (wallet == null)
                return Result.Failure(new Error("Wallet", "Wallet not found", ErrorType.NotFound));
            var conversionResult = command.Amount.ToMoneyDomain();
            if (conversionResult.IsFailure)
                return Result.Failure(conversionResult.Error);
            var ibanResult = IBAN.Create(command.Iban);
            if (ibanResult.IsFailure)
                return Result.Failure(ibanResult.Error);
            var result = wallet.RequestPayout(conversionResult.Value, ibanResult.Value);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            await UnitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
