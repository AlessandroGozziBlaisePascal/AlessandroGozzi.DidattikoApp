using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.DomainTests.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using FluentAssertions;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Event;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.OrderFolder
{
    public class OrderTests
    {
        private readonly CreditCard _validCard = CreditCard.Create("Mario", "Rossi", "12/28", "1234").Value;

        private List<OrderItem> CreateSampleItems()
        {
            var price1 = Money.Create(10m).Value;
            var price2 = Money.Create(20m).Value;

            return new List<OrderItem>
        {
            OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Book 1", price1, 2).Value, // 10 * 2 = 20
            OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Book 2", price2, 1).Value  // 20 * 1 = 20
        };
        }

        #region Create Tests

        [Fact]
        public void Create_ShouldCalculateTotalPriceCorrectly_ConsideringQuantities()
        {
            var items = CreateSampleItems();

            var result = Order.Create(Guid.NewGuid(), _validCard, items);

            result.IsSuccess.Should().BeTrue();
            result.Value.TotalPrice.Amount.Should().Be(40m); // 20 + 20
            result.Value.Status.Should().Be(OrderStatus.Placed);
            result.Value._domainEvents.Should().ContainSingle(e => e is OrderPlacedEvent);
        }

        [Fact]
        public void Create_ShouldFail_WhenCustomerIdIsEmpty()
        {
            var items = CreateSampleItems();

            var result = Order.Create(Guid.Empty, _validCard, items);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order customer");
        }

        [Fact]
        public void Create_ShouldFail_WhenPaymentDetailsIsNull()
        {
            var items = CreateSampleItems();

            var result = Order.Create(Guid.NewGuid(), null!, items);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order payment details");
        }

        [Fact]
        public void Create_ShouldFail_WhenItemsListIsEmpty()
        {
            var result = Order.Create(Guid.NewGuid(), _validCard, new List<OrderItem>());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order items");
        }

        #endregion

        #region State Transitions Tests

        [Fact]
        public void Confirm_ShouldSucceed_WhenStatusIsPlaced()
        {
            var order = Order.Create(Guid.NewGuid(), _validCard, CreateSampleItems()).Value;
            order._domainEvents.Clear();

            var result = order.Confirm();

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Confirmed);
            order._domainEvents.Should().ContainSingle(e => e is OrderConfirmedEvent);
        }

        [Fact]
        public void MarkAsPrepared_ShouldFail_WhenOrderIsNotConfirmed()
        {
            var order = Order.Create(Guid.NewGuid(), _validCard, CreateSampleItems()).Value;

            var result = order.MarkAsPrepared();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order status");
        }

        [Fact]
        public void Ship_ShouldSucceed_WhenOrderIsPreparedAndTrackingCodeIsValid()
        {
            var order = Order.Create(Guid.NewGuid(), _validCard, CreateSampleItems()).Value;
            order.Confirm();
            order.MarkAsPrepared();
            order._domainEvents.Clear();

            var result = order.Ship("TRACK12345");

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Shipped);
            order.TrackingCode.Should().Be("TRACK12345");
            order._domainEvents.Should().ContainSingle(e => e is OrderShippedEvent);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Ship_ShouldFail_WhenTrackingCodeIsInvalid(string? trackingCode)
        {
            var order = Order.Create(Guid.NewGuid(), _validCard, CreateSampleItems()).Value;
            order.Confirm();
            order.MarkAsPrepared();

            var result = order.Ship(trackingCode!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tracking code");
        }

        [Fact]
        public void MarkAsDelivered_ShouldSucceed_WhenOrderIsShipped()
        {
            var order = Order.Create(Guid.NewGuid(), _validCard, CreateSampleItems()).Value;
            order.Confirm();
            order.MarkAsPrepared();
            order.Ship("TRACK12345");
            order._domainEvents.Clear();

            var result = order.MarkAsDelivered();

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Delivered);
            order._domainEvents.Should().ContainSingle(e => e is OrderDeliveredEvent);
        }

        [Fact]
        public void CancelOrder_ShouldFail_WhenOrderIsShipped()
        {
            var order = Order.Create(Guid.NewGuid(), _validCard, CreateSampleItems()).Value;
            order.Confirm();
            order.MarkAsPrepared();
            order.Ship("TRACK12345");

            var result = order.CancelOrder();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order status");
        }

        [Fact]
        public void CancelOrder_ShouldSucceed_WhenOrderIsPlaced()
        {
            var order = Order.Create(Guid.NewGuid(), _validCard, CreateSampleItems()).Value;
            order._domainEvents.Clear();

            var result = order.CancelOrder();

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Cancelled);
            order._domainEvents.Should().ContainSingle(e => e is OrderCanceledEvent);
        }

        #endregion

    }
}
