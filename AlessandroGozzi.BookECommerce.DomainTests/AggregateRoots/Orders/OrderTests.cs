using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Orders;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Orders.Events;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Orders.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Shipments.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.AggregateRoots.Orders
{
    public class OrderTests
    {
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly PaymentDetails _paymentDetails = PaymentDetails.FromWallet(); // Instanza/Mock di PaymentDetails
        private readonly ShippingType _shippingType = ShippingType.Pieghe_Libri_Raccomandato;
        private const decimal ShippingFee = 4.80m;

        private OrderItem CreateSampleOrderItem(decimal priceAmount = 10m, int quantity = 1)
        {
            return OrderItem.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Sample Book",
                Money.Create(priceAmount).Value,
                ImageUrl.Create("cover.jpg").Value,
                quantity
            ).Value;
        }

        private Order CreateValidOrder(List<OrderItem>? items = null, decimal shippingFee = ShippingFee)
        {
            items ??= new List<OrderItem> { CreateSampleOrderItem(15m) };
            return Order.Create(_customerId, _paymentDetails, items, _shippingType, shippingFee).Value;
        }

        // ==========================================
        // CREATE TESTS
        // ==========================================
        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccessAndRaiseOrderPlacedEvent()
        {
            var items = new List<OrderItem> { CreateSampleOrderItem(15m) };

            var result = Order.Create(_customerId, _paymentDetails, items, _shippingType, ShippingFee);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.CustomerId.Should().Be(_customerId);
            result.Value.PaymentDetails.Should().Be(_paymentDetails);
            result.Value.ShippingType.Should().Be(_shippingType);
            result.Value.ShippingFee.Should().Be(ShippingFee);
            result.Value.Status.Should().Be(OrderStatus.Placed);
            result.Value.Items.Should().HaveCount(1);
            result.Value.TotalPrice.Amount.Should().Be(19.80m); // 15 + 4.80
            result.Value._domainEvents.Should().ContainSingle(e => e is OrderPlacedEvent);
        }

        [Fact]
        public void Create_WhenCustomerIdIsEmpty_ShouldReturnFailure()
        {
            var items = new List<OrderItem> { CreateSampleOrderItem() };

            var result = Order.Create(Guid.Empty, _paymentDetails, items, _shippingType, ShippingFee);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order customer");
            result.Error.Description.Should().Be("Customer ID cannot be empty");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenPaymentDetailsIsNull_ShouldReturnFailure()
        {
            var items = new List<OrderItem> { CreateSampleOrderItem() };

            var result = Order.Create(_customerId, null!, items, _shippingType, ShippingFee);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order payment details");
            result.Error.Description.Should().Be("Payment details cannot be null");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void Create_WhenItemsIsNullOrEmpty_ShouldReturnFailure(int count)
        {
            List<OrderItem>? items = count == 0 ? new List<OrderItem>() : null;

            var result = Order.Create(_customerId, _paymentDetails, items!, _shippingType, ShippingFee);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order items");
            result.Error.Description.Should().Be("OrderItems must contain at least one item");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenShippingFeeIsNegative_ShouldReturnFailure()
        {
            var items = new List<OrderItem> { CreateSampleOrderItem() };

            var result = Order.Create(_customerId, _paymentDetails, items, _shippingType, -1m);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order shipping fee");
            result.Error.Description.Should().Be("Shipping fee must be greater than or equal to zero");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        // ==========================================
        // FREE SHIPPING PROPERTY TESTS
        // ==========================================
        [Fact]
        public void IsFreeShippingApplied_WhenTotalPriceIsBelowThreshold_ShouldReturnFalse()
        {
            var items = new List<OrderItem> { CreateSampleOrderItem(20m) };

            var order = Order.Create(_customerId, _paymentDetails, items, _shippingType, 5m).Value;

            order.TotalPrice.Amount.Should().Be(25m); // < 30
            order.IsFreeShippingApplied.Should().BeFalse();
        }

        [Fact]
        public void IsFreeShippingApplied_WhenTotalPriceIsGreaterOrEqualToThreshold_ShouldReturnTrue()
        {
            var items = new List<OrderItem> { CreateSampleOrderItem(30m) };

            var order = Order.Create(_customerId, _paymentDetails, items, _shippingType, 0m).Value;

            order.TotalPrice.Amount.Should().Be(30m); // >= 30
            order.IsFreeShippingApplied.Should().BeTrue();
        }

        // ==========================================
        // CONFIRM TESTS
        // ==========================================
        [Fact]
        public void Confirm_WhenStatusIsPlaced_ShouldChangeStatusToConfirmedAndRaiseEvent()
        {
            var order = CreateValidOrder();
            order._domainEvents.Clear();

            var result = order.Confirm();

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Confirmed);
            order._domainEvents.Should().ContainSingle(e => e is OrderConfirmedEvent);
        }

        [Fact]
        public void Confirm_WhenStatusIsNotPlaced_ShouldReturnStatusConflictFailure()
        {
            var order = CreateValidOrder();
            order.Confirm(); // Imposta stato a Confirmed

            var result = order.Confirm();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order confirmation");
            result.Error.Description.Should().Be("The order is not in Placed status, cannot confirm");
            result.Error.Type.Should().Be(ErrorType.StatusConflict);
        }

        // ==========================================
        // ADD SHIPMENT TESTS
        // ==========================================
        [Fact]
        public void AddShipment_WhenOrderIsNotConfirmed_ShouldReturnStatusConflictFailure()
        {
            var order = CreateValidOrder(); // Stato: Placed
            var shipmentId = Guid.NewGuid();

            var result = order.AddShipment(shipmentId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order shipment");
            result.Error.Description.Should().Be("The order is not confirmed, can't add shipment");
            result.Error.Type.Should().Be(ErrorType.StatusConflict);
        }

        [Fact]
        public void AddShipment_WhenShipmentAlreadyExists_ShouldReturnStatusConflictFailure()
        {
            var order = CreateValidOrder();
            order.Confirm();
            var shipmentId = Guid.NewGuid();
            order.AddShipment(shipmentId);

            var result = order.AddShipment(shipmentId);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order shipment");
            result.Error.Description.Should().Be("Shipment id is already in the order");
            result.Error.Type.Should().Be(ErrorType.StatusConflict);
        }

        [Fact]
        public void AddShipment_WhenConfirmedAndNewShipment_ShouldAddShipmentId()
        {
            var order = CreateValidOrder();
            order.Confirm();
            var shipmentId = Guid.NewGuid();

            var result = order.AddShipment(shipmentId);

            result.IsSuccess.Should().BeTrue();
            order.ShipmentIds.Should().ContainSingle(id => id == shipmentId);
        }

        // ==========================================
        // CANCEL ORDER TESTS
        // ==========================================
        [Fact]
        public void CancelOrder_WhenNotCancelled_ShouldChangeStatusToCancelledAndRaiseEvent()
        {
            var order = CreateValidOrder();
            order._domainEvents.Clear();

            var result = order.CancelOrder();

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Cancelled);
            order._domainEvents.Should().ContainSingle(e => e is OrderCanceledEvent);
        }

        [Fact]
        public void CancelOrder_WhenAlreadyCancelled_ShouldReturnStatusConflictFailure()
        {
            var order = CreateValidOrder();
            order.CancelOrder();

            var result = order.CancelOrder();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order status");
            result.Error.Description.Should().Be("Order is already canceled");
            result.Error.Type.Should().Be(ErrorType.StatusConflict);
        }
    }
}
