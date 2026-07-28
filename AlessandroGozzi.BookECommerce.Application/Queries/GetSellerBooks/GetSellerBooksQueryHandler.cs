using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.GetSellerBooks
{
    public class GetSellerBooksQueryHandler: IRequestHandler<GetSellerBooksQuery, Result<IEnumerable<BookDto>>>
    {
        private readonly IBookRepository Repo;

        public GetSellerBooksQueryHandler(IBookRepository repo)
        {
            Repo = repo;
        }
        
        public async Task<Result<IEnumerable<BookDto>>> Handle(GetSellerBooksQuery request, CancellationToken token)
        {
            var books = await Repo.GetAllByCustomerIdAsync(request.sellerId, token);

            var dtos = books.Select(b => b.ToDto());

            return Result.Success(dtos);
        }
    }
}
