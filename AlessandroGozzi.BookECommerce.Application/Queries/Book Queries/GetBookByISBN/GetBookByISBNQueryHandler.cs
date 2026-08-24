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

namespace AlessandroGozzi.BookECommerce.Application.Queries.Book_Queries.GetBookByISBN
{
    public class GetBookByISBNQueryHandler: IRequestHandler<GetBookByISBNQuery, Result<CatalogBookDto>>
    {
        private readonly IBookRepository BookRepo;

        public GetBookByISBNQueryHandler(IBookRepository bookRepo)
        {
            BookRepo = bookRepo;
        }

        public async Task<Result<CatalogBookDto>> Handle(GetBookByISBNQuery request, CancellationToken cancellationToken)
        {
            var book = await BookRepo.GetByISBNAsync(request.Isbn, cancellationToken);
            if (book is null)
                return Result.Failure<CatalogBookDto>(new Error("Book","Book not found",ErrorType.NotFound));
            var bookSummary = new CatalogBookDto(
                book.Id,
                book.ISBNCode.ToDto(),
                book.Status.ToDto(),
                book.MainPhoto,
                book.Price.ToDto(),
                book.IsAvailable
            );
            return Result.Success(bookSummary);
        }
    }
}
