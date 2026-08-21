using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Event;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.ShipmentFolder.Value_Object;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder
{
    public class Order : Entity
    {
        private const decimal FreShipmentTheshold = 30m;
        public Guid CustomerId { get; init; }
        public DateTime Date { get; init; }
        public PaymentDetails PaymentDetails { get; init; }
        public Money TotalPrice { get; init; }
        public OrderStatus Status { get; private set; }
        public bool IsFreeShippingApplied => TotalPrice.Amount >= FreShipmentTheshold;


        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();


        private readonly List<Guid> _shipmentIds = new();
        public IReadOnlyCollection<Guid> ShipmentIds => _shipmentIds.AsReadOnly();
        public ShippingType ShippingType { get; private set; }
        public static Dictionary<ShippingType, decimal> ShippingCosts { get; private set; } = new()
            { 
            { ShippingType.Pieghi_Libri_Ordinario, 1.45m },
                { ShippingType.Pieghi_Libri_Raccomandato, 4.80m },
                { ShippingType.Express, 8m }
            };
        public decimal ShippingFee { get; private set; }

        private Order() { }

        private Order(Guid customerId, PaymentDetails paymentDetails, List<OrderItem> items, ShippingType type, decimal shippingFee)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Date = DateTime.UtcNow;
            PaymentDetails = paymentDetails;
            _items = items;
            TotalPrice = CalculateTotalPrice(_items) + Money.Create(ShippingFee).Value;
            Status = OrderStatus.Placed;
            ShippingType = type;
            ShippingFee = shippingFee;
        }

        public static Result<Order> Create(Guid customerId, PaymentDetails paymentDetails, List<OrderItem> items, ShippingType type, decimal shippingFee)
        {
            if (customerId == Guid.Empty)
                return Result.Failure<Order>(new Error("Order customer", "Customer ID cannot be empty", ErrorType.Validation));

            if (paymentDetails is null)
                return Result.Failure<Order>(new Error("Order payment details", "Payment details cannot be null", ErrorType.Validation));

            if (items is null || items.Count == 0)
                return Result.Failure<Order>(new Error("Order items", "OrderItems must contain at least one item", ErrorType.Validation));
            if(shippingFee < 0)
                return Result.Failure<Order>(new Error("Order shipping fee", "Shipping fee must be greater than or equal to zero", ErrorType.Validation));
            var order = new Order(customerId, paymentDetails, items, type, shippingFee);
            order.Raise(new OrderPlacedEvent(order.Id));

            return Result.Success(order);
        }

        public Result AddShipment(Guid shipmentId)
        {
            if(Status != OrderStatus.Confirmed)
                return Result.Failure(new Error("Order shipment", "The order is not confirmed, can't add shipment", ErrorType.StatusConflict));
            if(ShipmentIds.Contains(shipmentId))
                return Result.Failure(new Error("Order shipment", "Shipment id is already in the order", ErrorType.StatusConflict));

            _shipmentIds.Add(shipmentId);
            return Result.Success();
        }

        public Result Confirm()
        {
            if (Status != OrderStatus.Placed)
                return Result.Failure(new Error("Order confirmation", "The order is not in Placed status, cannot confirm", ErrorType.StatusConflict));

            Status = OrderStatus.Confirmed;
            Raise(new OrderConfirmedEvent(Id));

            return Result.Success();
        }

        private static Money CalculateTotalPrice(List<OrderItem> items)
        {
            decimal TotalAmount = items.Sum(item => item.Price.Amount);

            var moneyResult = Money.Create(TotalAmount);

            return moneyResult.IsSuccess
                ? moneyResult.Value
                : Money.Create(0).Value;
        }

        public Result CancelOrder()
        {
            if(Status == OrderStatus.Cancelled)
            {
                return Result.Failure(new Error("Order status", "Order is already canceled", ErrorType.StatusConflict));

            Status = OrderStatus.Cancelled;
            Raise(new OrderCanceledEvent(Id));

            return Result.Success();
        }
    }
}
