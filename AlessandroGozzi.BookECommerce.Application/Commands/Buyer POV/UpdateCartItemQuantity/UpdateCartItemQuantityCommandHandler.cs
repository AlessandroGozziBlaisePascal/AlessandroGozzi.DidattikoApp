using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Buyer_POV.UpdateCartItemQuantity
{
    public class UpdateCartItemQuantityCommandHandler: IRequestHandler<UpdateCartItemQuantityCommand, Result>
    {
        private readonly ICartRepository CartRepo;
        private readonly IUnitOfWork UnitOfWork;

        public UpdateCartItemQuantityCommandHandler(ICartRepository cartRepo, IUnitOfWork unitOfWork)
        {
            CartRepo = cartRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateCartItemQuantityCommand command, CancellationToken token)
        {
            var cart = await CartRepo.GetByCustomerIdAsync(command.CustomerId, token);
            if (cart == null)
                return Result.Failure(new Error("Cart", "Cart not found", ErrorType.NotFound));

            cart.UpdateItemQuantity(command.BookId, command.NewQuantity);

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
