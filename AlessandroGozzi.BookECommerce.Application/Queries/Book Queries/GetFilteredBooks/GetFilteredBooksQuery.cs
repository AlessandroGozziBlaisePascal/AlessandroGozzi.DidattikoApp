using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.BookInformation;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Book_Queries.GetFilteredBooks
{
    public record GetFilteredBooksQuery(
        string? Title = null,
        string? Publisher = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        string? Condition = null,
        string? Subject = null
        ): IRequest<Result<List<CatalogBookDto>>>;
}
