using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Repository;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Repository;
using MediatR;
using Stripe;

namespace AlessandroGozzi.BookECommerce.Application.Commands.CompleteCheckout
{
    public class CompleteCheckoutCommandHandler: IRequestHandler<CompleteCheckoutCommand, Result<OrderDto>>
    {
        private readonly ICartRepository _cartRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IShipmentRepository _shipmentRepo; 
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepo;
        private readonly IBookRepository _bookRepo;

        public CompleteCheckoutCommandHandler(
            ICartRepository cartRepo,
            IOrderRepository orderRepo,
            IShipmentRepository shipmentRepo,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork,
            ICustomerRepository customerRepo,
            IBookRepository bookRepo)
        {
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
            _shipmentRepo = shipmentRepo;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _customerRepo = customerRepo;
            _bookRepo = bookRepo;
        }

        public async Task<Result<OrderDto>> Handle(CompleteCheckoutCommand command, CancellationToken token)
        {
            var paymentDetails = await _paymentService.IsPaymentSuccessfulAsync(command.PaymentIntentId, token);
            if (paymentDetails.IsFailure)
            {
                return Result.Failure<OrderDto>(new Error("Payment", "Payment failed", ErrorType.Failure));
            }

            var cart = await _cartRepo.GetByIdAsync(command.CartId, token);
            if (cart == null)
            {
                return Result.Failure<OrderDto>(new Error("Cart", "Cart not found", ErrorType.Failure));
            }

            var bookIds = cart.GetItems.Select(i => i.BookId).ToList();
            var books = await _bookRepo.GetByIdsAsync(bookIds, token);

            var orderItems = new List<OrderItem>();
            foreach (var cartItem in cart.GetItems)
            {
                var book = books.First(b => b.Id == cartItem.BookId);
                var orderItemResult = OrderItem.Create(
                    book.Id,
                    book.SellerId,
                    book.Title,
                    book.Price,
                    cartItem.Quantity
                );

                if (orderItemResult.IsFailure)
                {
                    return Result.Failure<OrderDto>(new Error("Order item", "Order item failed to be generated", ErrorType.Failure));
                }

                orderItems.Add(orderItemResult.Value);
            }

            var customer = await _customerRepo.GetByIdAsync(command.CustomerId, token);
            if (customer == null || customer.CreditCard == null)
            {
                return Result.Failure<OrderDto>(new Error("Customer", "Customer not found or null credit card", ErrorType.Failure));
            }
                        var orderResult = Order.Create(command.CustomerId, customer.CreditCard, orderItems);
            if (orderResult.IsFailure)
            {
                return Result.Failure<OrderDto>(new Error("Order", "Order failed to be created", ErrorType.Failure));
            }

            var order = orderResult.Value;

            var vendorGroups = orderItems.GroupBy(item => item.SellerId);

            foreach (var vendorGroup in vendorGroups)
            {
                var vendorId = vendorGroup.Key;

                if (vendorId == command.CustomerId)
                {
                    return Result.Failure<OrderDto>(new Error(
                        "Order.SelfPurchase",
                        "Non puoi acquistare libri messi in vendita da te stesso.", ErrorType.StatusConflict
                    ));
                }

                var shipmentResult = Shipment.Create(order.Id, vendorId);
                if (shipmentResult.IsFailure)
                {
                    return Result.Failure<OrderDto>(shipmentResult.Error);
                }

                var shipment = shipmentResult.Value;

                _shipmentRepo.Add(shipment);

                order.AddShipment(shipment.Id);
            }

            await _orderRepo.AddAsync(order, token);

            cart.ClearCart();

            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success(order.ToDto());
        }

    }
}
