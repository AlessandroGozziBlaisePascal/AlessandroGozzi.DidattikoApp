using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.RemoveFromSale
{
    public class RemoveFromSaleCommandHandler: IRequestHandler<RemoveFromSaleCommand, Result<BookDto>>
    {
        private readonly IBookRepository BookRepo;
        private readonly ICustomerRepository CustRepo;
        private readonly IUnitOfWork UnitOfWork;

        public RemoveFromSaleCommandHandler(IBookRepository bookRepo, ICustomerRepository custRepo, IUnitOfWork unitOfWork)
        {
            BookRepo = bookRepo;
            CustRepo = custRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result<BookDto>> Handle(RemoveFromSaleCommand command, CancellationToken token)
        {
            var book = await BookRepo.GetByIdAsync(command.BookId, token);
            if (book == null)
            {
                return Result.Failure<BookDto>(new Error("Book","Book not found",ErrorType.NotFound));
            }

            var customer = await CustRepo.GetByIdAsync(command.CustomerId, token);
            if (customer == null)
            {
                return Result.Failure<BookDto>(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }

            var result = book.RemoveFromMarket(customer.Id);
            if (result.IsFailure)
            {
                return Result.Failure<BookDto>(new Error("Book", "Book cannot be remove", ErrorType.Failure));
            }

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success(book.ToDto());
        }
    }
}
