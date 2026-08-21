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
            var money = Money.Create(amount).Value;
            return OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Il Signore degli Anelli", money, 1).Value;
        }

            return new List<OrderItem>
        {
            var result = OrderItem.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Design Patterns",
                Money.Create(49.99m).Value,
                2
            );

            result.IsSuccess.Should().BeTrue();
            result.Value.BookTitle.Should().Be("Design Patterns");
            result.Value.Price.Amount.Should().Be(49.99m);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void OrderItem_Create_WithInvalidTitle_ShouldFail(string? invalidTitle)
        {
            var result = OrderItem.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                invalidTitle!,
                Money.Create(10m).Value,
                2
            );

        [Fact]
        public void Create_ShouldCalculateTotalPriceCorrectly_ConsideringQuantities()
        {
            var result = OrderItem.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Clean Code",
                null!,
                2
            );

            var result = Order.Create(Guid.NewGuid(), _validCard, items);

            result.IsSuccess.Should().BeTrue();
            result.Value.TotalPrice.Amount.Should().Be(40m); // 20 + 20
            result.Value.Status.Should().Be(OrderStatus.Placed);
            result.Value._domainEvents.Should().ContainSingle(e => e is OrderPlacedEvent);
        }
        [Fact]
        public void OrderItem_Create_WithInvalidQuantity_ShouldFail()
        {
            var result = OrderItem.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Clean Code",
                Money.Create(10m).Value,
                -1
            );

            result.IsFailure.Should().BeTrue();
        }

        #endregion

        #region Order Creation & Total Price Tests

        [Fact]
        public void Create_ShouldFail_WhenCustomerIdIsEmpty()
        {
            var items = CreateSampleItems();

            var result = Order.Create(customerId, card, items);

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

            var resultEmpty = Order.Create(Guid.NewGuid(), card, new List<OrderItem>());

            var resultNull = Order.Create(Guid.NewGuid(), card, null!);

            resultEmpty.IsFailure.Should().BeTrue();
            resultEmpty.Error.Code.Should().Be("Order items");

            resultNull.IsFailure.Should().BeTrue();
            resultNull.Error.Code.Should().Be("Order items");
        }

        #endregion

        #region State Transitions Tests

        [Fact]
        public void Confirm_ShouldSucceed_WhenStatusIsPlaced()
        {
            var card = GetValidCreditCard();
            var items = new List<OrderItem> { GetValidOrderItem() };
            var order = Order.Create(Guid.NewGuid(), card, items).Value;

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Confirmed);
        }

        [Fact]
        public void Ship_ShouldSucceed_WhenOrderIsPreparedAndTrackingCodeIsValid()
        {
            var order = Order.Create(Guid.NewGuid(), GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;
            order.Confirm(); 

            var result = order.Ship("TRACK12345");

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Shipped);
            order.TrackingCode.Should().Be("TRACK12345");
            order._domainEvents.Should().ContainSingle(e => e is OrderShippedEvent);
        }

        [Fact]
        public void CancelOrder_BeforeShipping_ShouldSucceed()
        {
            var order = Order.Create(Guid.NewGuid(), GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;

            var result = order.MarkAsDelivered();

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Delivered);
            order._domainEvents.Should().ContainSingle(e => e is OrderDeliveredEvent);
        }

        [Fact]
        public void CancelOrder_ShouldFail_WhenOrderIsShipped()
        {
            var order = Order.Create(Guid.NewGuid(), GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;
            order.Confirm();

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
