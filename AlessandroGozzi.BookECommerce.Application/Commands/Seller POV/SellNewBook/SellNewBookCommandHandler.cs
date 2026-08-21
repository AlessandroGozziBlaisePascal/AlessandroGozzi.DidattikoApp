using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.SellNewBook
{
    public class SellNewBookCommandHandler: IRequestHandler<SellNewBookCommand, Result>
    {
        private readonly IBookRepository BookRepo;
        private readonly ICustomerRepository CustomerRepo;
        private readonly IUnitOfWork UnitOfWork;

        public SellNewBookCommandHandler(IBookRepository bookRepo, ICustomerRepository customerRepo, IUnitOfWork unitOfWork)
        {
            BookRepo = bookRepo;
            CustomerRepo = customerRepo;
            UnitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SellNewBookCommand command, CancellationToken token)
        {
            var seller = CustomerRepo.GetByIdAsync(command.SellerId, token);
            if(seller == null)
            {
                return Result.Failure(new Error("Book seller", "Seller not found in DB", ErrorType.NotFound));
            }

            #region VO Result Check

            var subjectResult = command.Subject.ToSubjectDomain();
            if(subjectResult.IsFailure)
            {
                return Result.Failure(subjectResult.Error);
            }

            var isbnResult = command.Isbn.ToISBNDomain();
            if(isbnResult.IsFailure)
            {
                return Result.Failure(isbnResult.Error);
            }

            var priceResult = command.Price.ToMoneyDomain();
            if(priceResult.IsFailure)
            {
                return Result.Failure(priceResult.Error);
            }

            var conditionResult = command.Condition.ToDomain();
            if(conditionResult.IsFailure)
            {
                return Result.Failure(conditionResult.Error);
            }
            #endregion

            var book = Book.Create(
                command.Title,
                command.SellerId,
                subjectResult.Value,
                isbnResult.Value,
                command.SchoolYear,
                command.PublicationYear,
                priceResult.Value,
                conditionResult.Value
            );

            BookRepo.Add(book.Value);
            await UnitOfWork.SaveChangesAsync(token);   
            return Result.Success();
        }
    }
}
