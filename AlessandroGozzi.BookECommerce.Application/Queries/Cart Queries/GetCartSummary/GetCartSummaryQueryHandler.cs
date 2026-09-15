using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.Repositories;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Queries.Cart_Queries.GetCartSummary
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
            var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
            if (cart == null)
                return Result.Failure<CartSummaryDto>(new Error("Cart", "Cart not found", ErrorType.NotFound));
            var booksIds = cart.GetItems.Select(item => item.BookId).ToList();
            var books = await _bookRepository.GetByIdsAsync(booksIds, cancellationToken);

            var cartDto = cart.ToDto(books);
            var calculationResult = SmartCartGenerator.Calculate(cartDto.Items.ToList(), request.Type);
            var summaryDto = new CartSummaryDto(cartDto, calculationResult);
            return Result.Success(summaryDto);
        }
    }
}
