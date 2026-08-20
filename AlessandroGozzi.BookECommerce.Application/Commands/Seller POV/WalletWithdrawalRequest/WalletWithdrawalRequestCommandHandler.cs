using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Seller_POV.WalletWithdrawalRequest
{
    public class WalletWithdrawalRequestCommandHandler: IRequestHandler<WalletWithdrawalRequestCommand, Result>
    {
        private readonly ICustomerRepository CustomerRepository;
        private readonly IUnitOfWork UnitOfWork;

        public WalletWithdrawalRequestCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            CustomerRepository = customerRepository;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(WalletWithdrawalRequestCommand command, CancellationToken token)
        {
            if(command.Amount < 5m)
            {
                return Result.Failure(new Error("Withdrawal amount", "Min withdrawal amount is 5 €", ErrorType.Validation));
            }

            var customer = await CustomerRepository.GetByIdAsync(command.CustomerId, token);
            if(customer == null)
            {
                return Result.Failure(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }

            var conversionResult = command.Amount.ToMoneyDomain();
            if(conversionResult.IsFailure)
            {
                return Result.Failure(conversionResult.Error);
            }

            var result = customer.Wallet.Withdraw(conversionResult.Value);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            await UnitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
