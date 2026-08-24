using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.BooksRepository;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandHandler: IRequestHandler<RemoveFromCartCommand, Result>
    {
        private readonly ICartRepository CartRepo;
        private readonly IUnitOfWork UnitOfWork;

        public RemoveFromCartCommandHandler(ICartRepository cartRepo, IUnitOfWork unitOfWork)
        {
            CartRepo = cartRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveFromCartCommand command, CancellationToken token)
        {
            var cart = await CartRepo.GetByCustomerIdAsync(command.CustomerId, token);

            if (cart == null)
            {
                return Result.Failure(new Error("Cart", "Cart not found", ErrorType.NotFound));
            }

            var removeResult = cart.RemoveItem(command.BookId, command.Quantity);
            if(removeResult.IsFailure)
            {
                return Result.Failure(removeResult.Error);
            }

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
