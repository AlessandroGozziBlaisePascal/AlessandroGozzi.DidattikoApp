using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.Aggregate_Roots_Mappers;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Orders;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Orders.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments;
using AlessandroGozzi_BookECommerce.Domain.Repositories;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;
using MediatR;
using Stripe;

namespace AlessandroGozzi.BookECommerce.Application.Commands.CompleteCheckoutPayment
{
    public class CompleteCheckoutPaymentCommandHandler: IRequestHandler<CompleteCheckoutPaymentCommand, Result<OrderDto>>
    {
        private readonly ICartRepository _cartRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IShipmentRepository _shipmentRepo; 
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepo;
        private readonly IBookRepository _bookRepo;
        private readonly IWalletRepository walletRepository;

        public CompleteCheckoutPaymentCommandHandler(
            ICartRepository cartRepo,
            IOrderRepository orderRepo,
            IShipmentRepository shipmentRepo,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork,
            ICustomerRepository customerRepo,
            IBookRepository bookRepo,
            IWalletRepository walletRepository)
        {
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
            _shipmentRepo = shipmentRepo;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _customerRepo = customerRepo;
            _bookRepo = bookRepo;
            this.walletRepository = walletRepository;
        }

        public async Task<Result<OrderDto>> Handle(CompleteCheckoutPaymentCommand command, CancellationToken token)
        {
            if (command.PaymentIntentId != "WALLET_PAYMENT")
            {
                var paymentDetails = await _paymentService.IsPaymentSuccessfulAsync(command.PaymentIntentId, token);
                if (paymentDetails.IsFailure)
                {
                    return Result.Failure<OrderDto>(
                        new Error("Payment", "Payment failed", ErrorType.Failure));
                }
            }

            var cart = await _cartRepo.GetByIdAsync(command.CartId, token);
            if (cart == null)
            {
                return Result.Failure<OrderDto>(
                    new Error("Cart", "Cart not found", ErrorType.Failure));
            }

            var customer = await _customerRepo.GetByIdAsync(command.CustomerId, token);
            if (customer == null || customer.CreditCard == null)
            {
                return Result.Failure<OrderDto>(
                    new Error("Customer", "Customer not found or null credit card", ErrorType.Failure));
            }

            var bookIds = cart.GetItems.Select(i => i.BookId).ToList();
            var books = await _bookRepo.GetByIdsAsync(bookIds, token);
            var booksDict = books.ToDictionary(b => b.Id);

            var orderItems = new List<OrderItem>();
            foreach (var cartItem in cart.GetItems)
            {
                var book = books.First(b => b.Id == cartItem.BookId);
                var orderItemResult = OrderItem.Create(
                    book.Id,
                    book.SellerId,
                    book.Title,
                    book.Price,
                    book.MainPhoto,
                    cartItem.Quantity
                );

                if (orderItemResult.IsFailure)
                {
                    return Result.Failure<OrderDto>(
                        new Error("Order item", "Order item failed to be generated", ErrorType.Failure));
                }

                orderItems.Add(orderItemResult.Value);
            }

            var cartCalculation = SmartCartGenerator.Calculate(cart.ToDto(books).Items.ToList(), command.Type);

            
            decimal totalAmount = cartCalculation.GrandTotal;
            var wallet = await walletRepository.GetByCustomerId(command.CustomerId, token);
            if (wallet == null)
                return Result.Failure<OrderDto>(new Error("Wallet", "Wallet not found", ErrorType.NotFound));
            decimal availableWallet = wallet.AvailableBalance.Amount;
            decimal walletToDeduct = Math.Min(totalAmount, availableWallet);
            decimal cardAmount = totalAmount - walletToDeduct;

            if(cardAmount > 0 && cardAmount < 1)
            {
                cardAmount = 1m;
                walletToDeduct = totalAmount - cardAmount;
            }

            PaymentDetails orderPaymentDetails;
            if (walletToDeduct > 0 && cardAmount > 0)
                orderPaymentDetails = PaymentDetails.FromHybridPayment(customer.CreditCard.DisplayName);
            else if (walletToDeduct > 0)
                orderPaymentDetails = PaymentDetails.FromWallet();
            else
                orderPaymentDetails= PaymentDetails.FromCreditCard(customer.CreditCard.DisplayName);

            if (walletToDeduct > 0)
            {
                var walletDeductionResult = wallet.Withdraw(Money.Create(walletToDeduct).Value);
                if (walletDeductionResult.IsFailure)
                    return Result.Failure<OrderDto>(new Error("Wallet", "Failed to deduct from wallet", ErrorType.Failure));
            }
            var orderResult = Order.Create(
                command.CustomerId,
                orderPaymentDetails,
                orderItems,
                command.Type,
                cartCalculation.ShippingTotal
            );

            if (orderResult.IsFailure)
            {
                return Result.Failure<OrderDto>(
                    new Error("Order", "Order failed to be created", ErrorType.Failure));
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
                        "Non puoi acquistare libri messi in vendita da te stesso.",
                        ErrorType.StatusConflict
                    ));
                }

                decimal vendorSalesTotal = vendorGroup.Sum(i => i.Price.Amount * i.Quantity);
                if(vendorSalesTotal.ToMoneyDomain().IsFailure)
                {
                    return Result.Failure<OrderDto>(new Error(
                        "Order.VendorSalesTotal",
                        "Vendor sales total calculation failed.",
                        ErrorType.Failure
                    ));
                }
                var shipmentResult = Shipment.Create(
                    order.Id,
                    vendorId,
                    command.Type,
                    vendorSalesTotal.ToMoneyDomain().Value,
                    command.CustomerId,
                    customer.Address
                );

                if (shipmentResult.IsFailure)
                {
                    return Result.Failure<OrderDto>(shipmentResult.Error);
                }

                var shipment = shipmentResult.Value;
                _shipmentRepo.Add(shipment);
                order.AddShipment(shipment.Id);

                var vendor = await _customerRepo.GetByIdAsync(vendorId, token);
                if(vendor == null)
                {
                    return Result.Failure<OrderDto>(new Error("Vendor", $"Vendor with ID {vendorId} not found.",ErrorType.NotFound ));
                }

                var sellerWallet = await walletRepository.GetByCustomerId(vendorId, token);
                if (sellerWallet == null)
                    return Result.Failure<OrderDto>(new Error("Wallet", "Wallet not found", ErrorType.NotFound));
                sellerWallet.AddPendingFunds(vendorSalesTotal.ToMoneyDomain().Value);
            }

            _orderRepo.Add(order);
            cart.ClearCart();

            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success(order.ToDto());
        }

    }
}
