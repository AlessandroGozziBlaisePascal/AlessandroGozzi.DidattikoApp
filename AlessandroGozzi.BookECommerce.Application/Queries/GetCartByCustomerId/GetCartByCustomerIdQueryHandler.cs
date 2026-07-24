using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.GetCartByCustomerId
{
    public class GetCartByCustomerIdQueryHandler: IRequestHandler<GetCartByCustomerIdQuery, Result<CartDto>>
    {
        private readonly ICartRepository Repo;
        private readonly IBookRepository BookRepo;

        public GetCartByCustomerIdQueryHandler(ICartRepository repo, IBookRepository bookRepo)
        {
            Repo = repo;
            BookRepo = bookRepo;
        }

        public async Task<Result<CartDto>> Handle(GetCartByCustomerIdQuery request, CancellationToken token)
        {
            var cart = await Repo.GetByCustomerIdAsync(request.customerId, token);

            if(cart == null)
            {
               return Result.Failure<CartDto>(new Error("Customer Cart", "Cart bot found",ErrorType.NotFound)); 
            }

            var bookIds = cart.GetItems.Select(b => b.BookId);

            var books = await BookRepo.GetByIdsAsync(bookIds);

            return Result.Success(cart.ToDto(books));
        }
    }
}
