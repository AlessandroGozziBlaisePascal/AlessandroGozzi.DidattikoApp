using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Auth.RemoveCreditCard
{
    public class RemoveCreditCardCommandHandler: IRequestHandler<RemoveCreditCardCommand, Result>
    {
        private readonly ICustomerRepository CustomerRepo;
        private readonly IUnitOfWork UnitOfWork;

        public RemoveCreditCardCommandHandler(ICustomerRepository customerRepo, IUnitOfWork unitOfWork)
        {
            CustomerRepo = customerRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveCreditCardCommand command, CancellationToken token)
        {
            var customer = await CustomerRepo.GetByIdAsync(command.CustomerId, token);
            if (customer == null)
            {
                return Result.Failure(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }

            var result = customer.RemoveCreditCard();
            if (result.IsFailure)
                return Result.Failure(result.Error);

            await CustomerRepo.UpdateAsync(customer, token);
            await UnitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
