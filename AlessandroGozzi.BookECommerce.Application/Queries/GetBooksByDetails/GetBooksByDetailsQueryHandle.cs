using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.GetBooksByDetails
{
    public class GetBooksByDetailsQueryHandle: IRequestHandler<GetBooksByDetailsQuery, Result<IEnumerable<BookDto>>>
    {
        private readonly IBookRepository Repo;

        public GetBooksByDetailsQueryHandle(IBookRepository repo)
        {
            Repo = repo;
        }

        public async Task<Result<IEnumerable<BookDto>>> Handle(GetBooksByDetailsQuery request, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(request.Subject))
                return Result.Failure<IEnumerable<BookDto>>(new Error("Subject parameter", "Subject cannot be null", ErrorType.Validation));
            if(request.SchoolYear < 1 || request.SchoolYear > 5)
                return Result.Failure<IEnumerable<BookDto>>(new Error("SchoolYear parameter", "School year must be between 1 and 5", ErrorType.Validation));
            
            var books = await Repo.GetByBooksDetailsAsync(request.Subject, request.SchoolYear, request.Title, token);

            var dtos = books.Select(pair => pair.Item1.ToDto(pair.Item2));

            return Result.Success(dtos);
        }
    }
}
