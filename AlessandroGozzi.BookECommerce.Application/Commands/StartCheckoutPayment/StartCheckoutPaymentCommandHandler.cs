
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Repository;
using MediatR;
using Stripe;
using Stripe.V2;

namespace AlessandroGozzi.BookECommerce.Application.Commands.StartCheckoutPayment
{
    public class StartCheckoutPaymentCommandHandler: IRequestHandler<StartCheckoutPaymentCommand, Result<CheckoutPaymentResultDto>>
    {
        private readonly ICartRepository CartRepo;
        private readonly IBookRepository BookRepo;

        public StartCheckoutPaymentCommandHandler(ICartRepository cartRepo, IBookRepository bookRepo)
        {
            CartRepo = cartRepo;
            BookRepo = bookRepo;
        }

        public async Task<Result<CheckoutPaymentResultDto>> Handle(StartCheckoutPaymentCommand command, CancellationToken token)
        {
            var cart = await CartRepo.GetByCustomerIdAsync(command.CustomerId);

            if (cart == null)
            {
                return Result.Failure<CheckoutPaymentResultDto>(new Error("Cart", "Cart not found", ErrorType.NotFound));
            }

            if (cart.GetItems.Count == 0)
            {
                return Result.Failure<CheckoutPaymentResultDto>(new Error("Cart", "Cart is empty", ErrorType.StatusConflict));
            }
            var booksId = cart.GetItems.Select(b => b.Id).ToList();

            var books = await BookRepo.GetByIdsAsync(booksId, token);

            var bookPrices = books.ToDictionary(b => b.Id, b => b.Price);

            decimal totalAmount = 0;
            foreach (var item in cart.GetItems)
            {
                if (!bookPrices.TryGetValue(item.BookId, out var currentPrice))
                    return Result.Failure<CheckoutPaymentResultDto>(new Error("Book price", "Book price not found", ErrorType.NotFound));

                totalAmount += (currentPrice.Amount * item.Quantity);
            }

            long totalInCent = (long)(totalAmount * 100);

            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = totalInCent,
                    Currency = "eur",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        {"CustomerId ", command.CustomerId.ToString()},
                        { "CartId", cart.Id.ToString() }
                    }
                };

                var service = new PaymentIntentService();
                PaymentIntent paymentIntent = await service.CreateAsync(options, cancellationToken: token);

                var dto = new CheckoutPaymentResultDto(
                    paymentIntent.ClientSecret,
                    paymentIntent.Id,
                    totalInCent
                );

                return Result.Success(dto);
                    
            } catch(StripeException ex)
            {
                return Result.Failure<CheckoutPaymentResultDto>(new Error("Stripe error",ex.Message, ErrorType.Failure));
            }

        }
    }
}
