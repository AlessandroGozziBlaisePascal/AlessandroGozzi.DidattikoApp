using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Buyer_POV.ClearCart
{
    public class ClearCartCommandHandler: IRequestHandler<ClearCartCommand, Result>
    {
        private readonly ICartRepository CartRepo;
        private readonly IUnitOfWork UnitOfWork;

        public ClearCartCommandHandler(ICartRepository cartRepo, IUnitOfWork unitOfWork)
        {
            CartRepo = cartRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ClearCartCommand command, CancellationToken token)
        {
            var cart = await CartRepo.GetByCustomerIdAsync(command.CustomerId, token);
            if (cart == null)
            {
                return Result.Failure(new Error("Cart", "Cart not found", ErrorType.NotFound));
            }

            cart.ClearCart();

            await UnitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
