using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.BookInformation;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Book_Queries.GetFilteredBooks
{
    public class GetFilteredBooksQueryHandler: IRequestHandler<GetFilteredBooksQuery, Result<List<CatalogBookDto>>>
    {
        private readonly IBookRepository _bookRepository;

        public GetFilteredBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Result<List<CatalogBookDto>>> Handle(GetFilteredBooksQuery request, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetByBooksDetailsAsync(
                request.Title,
                request.Publisher,
                request.MinPrice,
                request.MaxPrice,
                request.Condition,
                request.Subject,
                cancellationToken
            );

            var bookDtos = books.Select(book => new CatalogBookDto(
                book.Id,
                book.ISBNCode.ToDto(),
                book.Status.ToDto(),
                book.MainPhoto,
                book.Price.ToDto(),
                book.IsAvailable
            )).ToList();

            return Result.Success(bookDtos);
        }
    }
}
