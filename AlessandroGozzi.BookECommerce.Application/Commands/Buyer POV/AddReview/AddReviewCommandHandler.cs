using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Buyer_POV.AddReview
{
    public class AddReviewCommandHandler: IRequestHandler<AddReviewCommand, Result>
    {
        private readonly IBookRepository BookRepo;
        private readonly IUnitOfWork UnitOfWork;
        private readonly ICustomerRepository CustRepo;

        public AddReviewCommandHandler(IBookRepository bookRepo, IUnitOfWork unitOfWork, ICustomerRepository custRepo)
        {
            BookRepo = bookRepo;
            UnitOfWork = unitOfWork;
            CustRepo = custRepo;
        }

        public async Task<Result> Handle(AddReviewCommand command, CancellationToken token)
        {
            var book = await BookRepo.GetByIdAsync(command.BookId, token);
            if (book == null)
            {
                return Result.Failure(new Error("Book", "Book not found", ErrorType.NotFound));
            }

            var customer = await CustRepo.GetByIdAsync(command.CustomerId, token);
            if(customer == null)
            {
                return Result.Failure(new Error("Customer", "Customer not found", ErrorType.NotFound));
            }
            var reviewResult = BookReview.Create(command.CustomerId, customer.FullName, command.Rating);
            if (reviewResult.IsFailure)
            {
                return Result.Failure(reviewResult.Error);
            }
            book.AddReview(reviewResult.Value);

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
