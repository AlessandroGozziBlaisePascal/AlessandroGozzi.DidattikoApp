using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.GetBooksByDetails
{
    public sealed record GetBooksByDetailsQuery(string Subject, int SchoolYear, string? Title = null): IRequest<Result<IEnumerable<BookDto>>>;
}
