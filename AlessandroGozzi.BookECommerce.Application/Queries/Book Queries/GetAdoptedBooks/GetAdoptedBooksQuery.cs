using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.BookSearching;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Book_Queries.GetAdoptedBooks
{
    public record GetAdoptedBooksQuery(
    string SchoolCode,
    int Grade,
    string Section,
    string AcademicYear
    ) : IRequest<Result<ClassAdoptionResponseDto>>;
}


