using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Buyer_POV.Request.TopUpWallet
{
    public class TopUpWalletRequestCommandHandler: IRequestHandler<TopUpWalletRequestCommand, Result>
    {
        private readonly ICustomerRepository CustomerRepo;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IPaymentService _paymentService;

        public TopUpWalletRequestCommandHandler(ICustomerRepository customerRepo, IUnitOfWork unitOfWork, IPaymentService payService)
        {
            CustomerRepo = customerRepo;
            UnitOfWork = unitOfWork;
            _paymentService = payService;
        }

        public async Task<Result> Handle(TopUpWalletRequestCommand command, CancellationToken token)
        {
            if (command.Amount < 1)
                return Result.Failure(new Error("Amount", "Amount must be greater than 1", ErrorType.Validation));

            var customer = await CustomerRepo.GetByIdAsync(command.CustomerId, token);
            if(customer == null)
            {
                return Result.Failure(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }

            if(customer.CreditCard == null)
            {
                return Result.Failure(new Error("Credit card", "Card not found", ErrorType.NotFound));
            }

            if(customer.CreditCard.IsExpired())
                return Result.Failure(new Error("Credit card","Credit card expired",ErrorType.Failure));

            return await _paymentService.CreatePaymentIntentAsync(command.Amount, command.CustomerId, Guid.Empty, token);
        }
    }
}
