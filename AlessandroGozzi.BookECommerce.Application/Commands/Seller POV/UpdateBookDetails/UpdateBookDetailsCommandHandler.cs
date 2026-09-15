using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.Seller_POV.UpdateBookDetails
{
    public class UpdateBookDetailsCommandHandler : IRequestHandler<UpdateBookDetailsCommand, Result<BookDto>>
    {
        private readonly IBookRepository BookRepo;
        private readonly IUnitOfWork UnitOfWork;
        private readonly ICustomerRepository CustRepo;

        public UpdateBookDetailsCommandHandler(IBookRepository bookRepo, ICustomerRepository cRepo, IUnitOfWork unitOfWork)
        {
            BookRepo = bookRepo;
            CustRepo = cRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result<BookDto>> Handle(UpdateBookDetailsCommand command, CancellationToken token)
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

            if (command.NewPrice.HasValue)
            {
                var priceResult = command.NewPrice.Value.ToMoneyDomain();
                if (priceResult.IsFailure)
                {
                    return Result.Failure<BookDto>(new Error("Book new price", "Invalid new price", ErrorType.Validation));
                }
                var result = book.UpdatePrice(command.NewPrice.Value.ToMoneyDomain().Value, customer.Id);
                if (result.IsFailure)
                {
                    return Result.Failure<BookDto>(new Error("Book new price", "Failed to change book price", ErrorType.Failure));
                }
            }
            if (!string.IsNullOrWhiteSpace(command.NewStatus))
            {
                var statusResult = command.NewStatus.ToBookStatusDomain();
                if (statusResult.IsFailure)
                {
                    return Result.Failure<BookDto>(new Error("Book new status", "Invalid new status", ErrorType.Validation));
                }
                var result = book.UpdateStatus(command.NewStatus.ToBookStatusDomain().Value, customer.Id);
                if (result.IsFailure)
                {
                    return Result.Failure<BookDto>(new Error("Book new status", "Failed to change book status", ErrorType.Failure));
                }
            }
            if (command.NewPhoto != null)
            {
                var urlConvertionResult = command.NewPhoto.ToImageUrlDomain();
                if(urlConvertionResult.IsFailure)
                {
                    return Result.Failure<BookDto>(urlConvertionResult.Error);
                }
                var result = book.ChangePhoto(command.CustomerId, urlConvertionResult.Value);
                if (result.IsFailure)
                {
                    return Result.Failure<BookDto>(new Error("Book new photos", "Failed to change book photos", ErrorType.Failure));
                }
            }

            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success(book.ToDto());
        }
    }
}
