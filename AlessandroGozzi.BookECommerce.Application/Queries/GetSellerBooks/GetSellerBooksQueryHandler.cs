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
        private readonly ICustomerRepository CustomerRepo;

        public GetSellerBooksQueryHandler(IBookRepository repo, ICustomerRepository customerRepository)
        {
            Repo = repo;
            CustomerRepo = customerRepository;
        }
        
        public async Task<Result<IEnumerable<BookDto>>> Handle(GetSellerBooksQuery request, CancellationToken token)
        {
            var seller = await CustomerRepo.GetByIdAsync(request.sellerId, token);
            if(seller == null)
            {
                return Result.Failure<IEnumerable<BookDto>>(new Error("Book seller","Seller not found",ErrorType.NotFound));
            }
            var books = await Repo.GetAllByCustomerIdAsync(request.sellerId, token);

            var dtos = books.Select(b => b.ToDto(seller));

            return Result.Success(dtos);
        }
    }
}
