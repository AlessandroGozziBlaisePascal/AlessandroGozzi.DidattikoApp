using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.BookInformation;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.GetBookByISBN
{
    public record GetBookByISBNQuery(string Isbn): IRequest<Result<CatalogBookDto>>;
}
