using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.AddToCart
{
    public class AddToCartCommandHandler: IRequestHandler<AddToCartCommand, Result>
    {
        private readonly ICartRepository CartRepo;
        private readonly IBookRepository BookRepo;

        public AddToCartCommandHandler(ICartRepository cartRepo, IBookRepository bookRepo)
        {
            CartRepo = cartRepo;
            BookRepo = bookRepo;
        }

        public async Task<Result> Handle(AddToCartCommand command, CancellationToken cancellationToken)
        {
            var cart = await CartRepo.GetByCustomerIdAsync(command.CustomerId, cancellationToken);

            if (cart == null)
            {
                return Result.Failure(new Error("Cart", "Cart not found", ErrorType.NotFound));
            }

            var book = await BookRepo.GetByIdAsync(command.BookId, cancellationToken);
            if(book == null)
            {
                return Result.Failure(new Error("Book", "Book not found in DB", ErrorType.NotFound));
            }

            cart.AddItem(command.BookId, book.Title, book.Price, book.MainPhoto, command.quantity);

            await CartRepo.UpdateAsync(cart, cancellationToken);

            return Result.Success();
            
        }
    }
}
