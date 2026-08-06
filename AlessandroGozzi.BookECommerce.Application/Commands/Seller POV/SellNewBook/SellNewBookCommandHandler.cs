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

        public SellNewBookCommandHandler(IBookRepository bookRepo, ICustomerRepository customerRepo)
        {
            BookRepo = bookRepo;
            CustomerRepo = customerRepo;
        }

        public async Task<Result> Handle(SellNewBookCommand command, CancellationToken token)
        {
            var seller = CustomerRepo.GetByIdAsync(command.SellerId, token);
            if(seller == null)
            {
                return Result.Failure(new Error("Book seller", "Seller not found in DB", ErrorType.NotFound));
            }

            var book = Book.Create(
                command.Title,
                command.SellerId,
                command.Subject.ToSubjectDomain(),
                command.Isbn.ToISBNDomain(),
                command.SchoolYear,
                command.PublicationYear,
                command.Price.ToMoneyDomain(),
                command.Condition.ToDomain()
            );

            await BookRepo.AddAsync(book.Value, token);
            return Result.Success();
        }
    }
}
