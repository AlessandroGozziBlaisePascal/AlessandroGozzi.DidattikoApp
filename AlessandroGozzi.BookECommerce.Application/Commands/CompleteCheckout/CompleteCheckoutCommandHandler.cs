using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using Stripe;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Repository;
using MediatR;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder;

namespace AlessandroGozzi.BookECommerce.Application.Commands.CompleteCheckout
{
    public class CompleteCheckoutCommandHandler: IRequestHandler<CompleteCheckoutCommand, Result<OrderDto>>
    {
        private readonly ICartRepository CartRepo;
        private readonly IOrderRepository OrderRepo;
        private readonly IPaymentService PaymentService;
        private readonly IUnitOfWork UnitOfWork;
        private readonly IBookRepository BookRepo;

        public CompleteCheckoutCommandHandler(ICartRepository cartRepo, IOrderRepository orderRepo, IPaymentService service, IUnitOfWork unitOfWork, IBookRepository bookRepo)
        {
            CartRepo = cartRepo;
            OrderRepo = orderRepo;
            PaymentService = service;
            UnitOfWork = unitOfWork;
            BookRepo = bookRepo;
        }

        public async Task<Result<OrderDto>> Handle(CompleteCheckoutCommand command, CancellationToken token)
        {
            var paymentDetails = await PaymentService.IsPaymentSuccessful(command.PaymentIntentId, token);

            if (!paymentDetails.Item1)
            {
                return Result.Failure<OrderDto>(new Error("Payment", "Payment failed", ErrorType.Failure));
            }

            var cart = await CartRepo.GetByIdAsync(command.CartId, token);
            if(cart == null)
            {
                return Result.Failure<OrderDto>(new Error("Cart", "Cart not found", ErrorType.Failure));
            }

            var bookIds = cart.GetItems.Select(i => i.BookId).ToList();
            var books = await BookRepo.GetByIdsAsync(bookIds, token);
            var orderItems = new List<OrderItem>();
            foreach(var cartItem in cart.GetItems)
            {
                var book = books.First(b => b.Id == cartItem.BookId);
                var orderItemResult = OrderItem.Create(
                    book.Id,
                    book.SellerId,
                    book.Title,
                    book.Price,
                    cartItem.Quantity);
                if (orderItemResult.IsFailure)
                {
                    return Result.Failure<OrderDto>(new Error("Order item", "Order item failed to be generated", ErrorType.Failure));
                }
                orderItems.Add(orderItemResult.Value);
            }

            var order = Order.Create(command.CustomerId, paymentDetails.Item2, orderItems);
            if(order.IsFailure)
                return Result.Failure<OrderDto>(new Error("Order", "Order failed to be created", ErrorType.Failure));

            await OrderRepo.AddAsync(order.Value, token);
            cart.ClearCart();
            await CartRepo.UpdateAsync(cart, token);
            
            await UnitOfWork.SaveChangesAsync(token);

            return Result.Success(order.Value.ToDto());
        }
    }
}
