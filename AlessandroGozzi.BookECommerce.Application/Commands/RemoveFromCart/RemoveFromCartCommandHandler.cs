using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandHandler: IRequestHandler<RemoveFromCartCommand, Result>
    {
        private readonly ICartRepository CartRepo;
        private readonly IBookRepository BookRepo;

        public RemoveFromCartCommandHandler(ICartRepository cartRepo, IBookRepository bookRepo)
        {
            CartRepo = cartRepo;
            BookRepo = bookRepo;
        }

        public async Task<Result> Handle(RemoveFromCartCommand command, CancellationToken token)
        {
            var cart = await CartRepo.GetByCustomerIdAsync(command.CustomerId, token);

            if (cart == null)
            {
                return Result.Failure(new Error("Cart", "Cart not found", ErrorType.NotFound));
            }

            var book = await BookRepo.GetByIdAsync(command.BookId, token);
            if (book == null)
            {
                return Result.Failure(new Error("Book", "Book not found in DB", ErrorType.NotFound));
            }

            cart.RemoveBook(command.BookId, command.quantity);

            await CartRepo.UpdateAsync(cart, token);

            return Result.Success();
        }
    }
}
