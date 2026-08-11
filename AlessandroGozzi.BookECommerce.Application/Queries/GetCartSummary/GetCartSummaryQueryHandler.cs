using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.GetCartSummary
{
    public class GetCartSummaryQueryHandler: IRequestHandler<GetCartSummaryQuery, Result<CartSummaryDto>>   
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBookRepository _bookRepository;
        public GetCartSummaryQueryHandler(ICartRepository cartRepository, IBookRepository bookRepo)
        {
            _cartRepository = cartRepository;
            _bookRepository = bookRepo;
        }

        public async Task<Result<CartSummaryDto>> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(req, cancellationToken);
            if (cart == null)
                return Result.Failure<CartSummaryDto>(new Error("Cart", "Cart not found", ErrorType.NotFound));
            var booksIds = cart.GetItems.Select(item => item.BookId).ToList();
            var books = await _bookRepository.GetByIdsAsync(booksIds, cancellationToken);

            var cartDto = cart.ToDto(books);
            var calculationResult = SmartCartGenerator.Calculate(car), request.Type);
            if (calculationResult.IsFailure)
                return Result.Failure<CartSummaryDto>(calculationResult.Error);
            var summaryDto = new CartSummaryDto(cart.ToDto(), calculationResult.Value);
            return Result.Success(summaryDto);
        }
    }
}
