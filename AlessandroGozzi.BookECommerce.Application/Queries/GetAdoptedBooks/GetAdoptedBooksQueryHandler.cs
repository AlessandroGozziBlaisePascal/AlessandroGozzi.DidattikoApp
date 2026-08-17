using System;
using AlessandroGozzi.BookECommerce.Application.Dto.BookSearching;
using AlessandroGozzi.BookECommerce.SharedKernel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.GetAdoptedBooks
{
    public class GetAdoptedBooksQueryHandler: IRequestHandler<GetAdoptedBooksQuery, Result<ClassAdoptionResponseDto>>
    {
        private readonly IMinistryApiClient _ministryApiClient;
        private readonly DbContext _dbContext;
        public GetAdoptedBooksQueryHandler(IMinistryApiClient ministryApiClient, DbContext dbcontext)
        {
            _ministryApiClient = ministryApiClient;
            _dbContext = dbcontext;
        }
        public async Task<Result<ClassAdoptionResponseDto>> Handle(GetAdoptedBooksQuery request, CancellationToken cancellationToken)
        {
            var result = await _ministryApiClient.GetAdoptedBooksAsync(request.SchoolCode, request.Grade, request.Section, request.AcademicYear);
            if (result.IsFailure)
            {
                return Result.Failure<List<ClassAdoptionResponseDto>>(result.Error);
            }

            var ministryData = result.Value;

            var isbns = ministryData.Items.Select(x => x.ISBN).ToList();

            var platformOffers = await _dbContext.Set<Book>().AsNoTracking()
                .Where(b => isbns.Contains(b.ISBN) && b.IsAvailable)
                .GroupBy(b => b.ISBN)
                .Select(g => new
                {
                    ISBN = g.Key,
                    SellersCount = g.Count(),   
                }).ToDictionaryAsync(x => x.ISBN, cancellationToken);

            var mappedBooks = ministryData.Items.Select(item =>
            {
                platformOffers.TryGetValue(item.ISBN, out var offer);
                return new AdoptedBookItemDto(
                    item.ISBN,
                    item.Title,
                    item.Author,
                    item.Publisher,
                    item.Subject,
                    item.Price,
                    item.IsMandatory,
                    offer != null && offer.SellersCount > 0
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
