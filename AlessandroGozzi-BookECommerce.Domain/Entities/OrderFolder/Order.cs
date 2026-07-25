using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Event;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder
{
    public class Order: Entity
    {
        public Guid CustomerId { get; init; }
        public DateTime Date { get; init; }
        public CreditCard PaymentDetails { get; init; }
        public Money TotalPrice { get; init; }
        public OrderStatus Status { get; private set; }
        public string? TrackingCode { get; private set;}

        private readonly List<OrderItem> _items = new();

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { }

        private Order(Guid customerId, CreditCard paymentDetails, List<OrderItem> items)
        {
            CustomerId = customerId;
            PaymentDetails = paymentDetails;
            _items = items;
            TotalPrice = CalculateTotalPrice(_items);
            Status = OrderStatus.Placed;
        }

        private static Money CalculateTotalPrice(List<OrderItem> items)
        {
            decimal TotalAmount = items.Sum(item => item.Price.Amount);

            var moneyResult = Money.Create(TotalAmount);

            return moneyResult.IsSuccess
                ? moneyResult.Value
                : Money.Create(0).Value;
        }

        public static Result<Order> Create(Guid customerId, CreditCard paymentDetails, List<OrderItem> items)
        {
            if(paymentDetails  == null)
            {
                return Result.Failure<Order>(new Error("Order payment details", "Payment details cant be null", ErrorType.Validation));
            }
            if(items == null || items.Count == 0)
                return Result.Failure<Order>(new Error("Order items", "OrderItems must contain at least one item", ErrorType.Validation));
            var order = new Order(customerId, paymentDetails, items);
            order.Raise(new OrderPlacedEvent(order.Id));
            return Result.Success(order);
        }

        public Result Confirm()
        {
            if(Status != OrderStatus.Placed)
                return Result.Failure(new Error("Order confirmation", "The order is not places, can't confirm", ErrorType.StatusConflict));
            Status = OrderStatus.Confirmed;
            Raise(new OrderConfirmedEvent(Id));
            return Result.Success();
        }
        public Result MarkAsPrepared()
        {
            if(Status != OrderStatus.Confirmed)
                return Result.Failure(new Error("Order status", "The order is not confirmed yet, can't be prepared", ErrorType.StatusConflict));
            Status = OrderStatus.Prepared;
            Raise(new OrderPreparedEvent(Id));
            return Result.Success();
        }

        public Result Ship(string trackingCode)
        {
            if (Status != OrderStatus.Prepared)
                return Result.Failure(new Error("Order shipping", "The order is not confirmed, can't ship", ErrorType.StatusConflict));
            
            if (trackingCode == null)
                return Result.Failure(new Error("Tracking code", "Tracking code is null", ErrorType.Validation));
            Status = OrderStatus.Shipped;
            TrackingCode = trackingCode;
            Raise(new OrderShippedEvent(Id, TrackingCode));
            return Result.Success();
        }

        public Result MarkAsDelivered()
        {
            if (Status != OrderStatus.Shipped)
                return Result.Failure(new Error("Order status", "Order can be delivered only if it got shipped", ErrorType.StatusConflict));
            Status = OrderStatus.Delivered;
            Raise(new OrderDeliveredEvent(Id));
            return Result.Success();
        }

        public Result CancelOrder()
        {
            if(TrackingCode != null)
            {
                return Result.Failure(new Error("Order status", "Order cannot be canceled because it got already shipped", ErrorType.StatusConflict));
            }
            if(Status == OrderStatus.Cancelled)
            {
                return Result.Failure(new Error("Order status", "Order is already canceled", ErrorType.StatusConflict));
            }

            Status = OrderStatus.Cancelled;
            Raise(new OrderCanceledEvent(Id));
            return Result.Success();
        }
    }
}
