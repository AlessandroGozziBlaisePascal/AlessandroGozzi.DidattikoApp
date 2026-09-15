using System;
using AlessandroGozzi.BookECommerce.Application.Dto.BookSearching;
using AlessandroGozzi.BookECommerce.SharedKernel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books;
using AlessandroGozzi.BookECommerce.Domain.Repositories;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Book_Queries.GetAdoptedBooks
{
    public class GetAdoptedBooksQueryHandler: IRequestHandler<GetAdoptedBooksQuery, Result<ClassAdoptionResponseDto>>
    {
        private readonly IMinistryApiClient _ministryApiClient;
        private readonly IBookRepository _bookRepository;

        public GetAdoptedBooksQueryHandler(
            IMinistryApiClient ministryApiClient,
            IBookRepository bookRepository)
        {
            _ministryApiClient = ministryApiClient;
            _bookRepository = bookRepository;
        }

        public async Task<Result<ClassAdoptionResponseDto>> Handle(GetAdoptedBooksQuery request, CancellationToken cancellationToken)
        {
            var result = await _ministryApiClient.GetAdoptedBooksAsync(
                request.SchoolCode,
                request.Grade,
                request.Section,
                request.AcademicYear);

            if (result.IsFailure)
            {
                return Result.Failure<ClassAdoptionResponseDto>(result.Error);
            }

            var ministryData = result.Value;
            var isbns = ministryData.Books.Select(x => x.ISBN).ToList();

            var platformOffers = await _bookRepository
                .GetAvailableSellersCountByIsbnsAsync(isbns, cancellationToken);

            var mappedBooks = ministryData.Books.Select(item =>
            {
                platformOffers.TryGetValue(item.ISBN, out var sellersCount);

                return new AdoptedBookItemDto(
                    item.ISBN,
                    item.Title,
                    item.Authors,
                    item.Publisher,
                    item.Subject,
                    item.Price,
                    item.IsMandatory,
                    sellersCount > 0
                );
            }).ToList();

            var response = new ClassAdoptionResponseDto(
                ministryData.SchoolName,
                ministryData.ClassName,
                ministryData.AcademicYear,
                mappedBooks
            );

            return Result.Success(response);
        }
    }
}
