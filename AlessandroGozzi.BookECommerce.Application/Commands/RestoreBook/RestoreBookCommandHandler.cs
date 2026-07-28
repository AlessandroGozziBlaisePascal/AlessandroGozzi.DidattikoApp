using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Commands.RemoveFromSale;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.RestoreBook
{
    public class RestoreBookCommandHandler: IRequestHandler<RestoreBookCommand, Result<BookDto>>
    {
        private readonly IBookRepository BookRepo;
        private readonly ICustomerRepository CustRepo;

        public RestoreBookCommandHandler(IBookRepository bookRepo, ICustomerRepository custRepo)
        {
            BookRepo = bookRepo;
            CustRepo = custRepo;
        }

        public async Task<Result<BookDto>> Handle(RestoreBookCommand command, CancellationToken token)
        {
            var book = await BookRepo.GetByIdAsync(command.BookId, token);
            if (book == null)
            {
                return Result.Failure<BookDto>(new Error("Book", "Book not found", ErrorType.NotFound));
            }

            var customer = await CustRepo.GetByIdAsync(command.CustomerId, token);
            if (customer == null)
            {
                return Result.Failure<BookDto>(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }

            var result = book.RestoreInMarket(customer.Id);
            if (result.IsFailure)
            {
                return Result.Failure<BookDto>(new Error("Book", "Book cannot be restored", ErrorType.Failure));
            }

            await BookRepo.UpdateAsync(book, token);

            return Result.Success(book.ToDto());
        }
    }
}
