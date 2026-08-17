
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.Checkout;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Repository;
using MediatR;
using Stripe;
using Stripe.V2;

namespace AlessandroGozzi.BookECommerce.Application.Commands.StartCheckoutPayment
{
    public class StartCheckoutPaymentCommandHandler: IRequestHandler<StartCheckoutPaymentCommand, Result<CheckoutPaymentResultDto>>
    {
        private readonly ICartRepository CartRepo;
        private readonly ICustomerRepository CustomerRepo;
        private readonly IBookRepository BookRepo;
        private readonly IPaymentService PaymentService;

        public StartCheckoutPaymentCommandHandler(ICartRepository cartRepo, IBookRepository bookRepo, IPaymentService service, ICustomerRepository custRepo)
        {
            CartRepo = cartRepo;
            BookRepo = bookRepo;
            PaymentService = service;
            CustomerRepo = custRepo;
        }

        public async Task<Result<CheckoutPaymentResultDto>> Handle(StartCheckoutPaymentCommand command, CancellationToken token)
        {
            var customer = await CustomerRepo.GetByIdAsync(command.CustomerId, token);
            if (customer == null || customer.CreditCard == null)
            {
                return Result.Failure<CheckoutPaymentResultDto>(new Error("Customer", "Customer not found or null credit card", ErrorType.Failure));
            }

            var cart = await CartRepo.GetByCustomerIdAsync(command.CustomerId, token);
            if (cart == null)
            {
                return Result.Failure<CheckoutPaymentResultDto>(new Error("Cart", "Cart not found", ErrorType.NotFound) );
            }

            if (cart.GetItems.Count == 0)
            {
                return Result.Failure<CheckoutPaymentResultDto>(new Error("Cart", "Cart is empty", ErrorType.StatusConflict));
            }

            var booksId = cart.GetItems.Select(b => b.BookId).ToList();
            var books = await BookRepo.GetByIdsAsync(booksId, token);

            var calculationResult = SmartCartGenerator.Calculate(cart.ToDto(books).Items.ToList(), command.Type);

            decimal grandTotal = calculationResult.GrandTotal;
            decimal walletBalance = customer.Wallet.AvailableBalance.Amount;

            decimal amountToChargeOnCard = grandTotal > walletBalance 
                ? grandTotal - walletBalance 
                : 0m;

            if(amountToChargeOnCard == 0m)
                return Result.Success(new CheckoutPaymentResultDto
                    (
                        ClientSecret: "WALLET_COVERED",
                        PaymentIntentId: "WALLET_PAYMENT",
                        Amount: (long)grandTotal
                    ));


            long totalInCent = (long)(calculationResult.GrandTotal * 100);

            var paymentCreationResult = await PaymentService.CreatePaymentIntentAsync(
                totalInCent,
                command.CustomerId,
                cart.Id,
                token
            );

            if (paymentCreationResult.IsFailure)
            {
                return Result.Failure<CheckoutPaymentResultDto>(paymentCreationResult.Error);
            }

            return Result.Success(paymentCreationResult.Value);
        }

    }
}
