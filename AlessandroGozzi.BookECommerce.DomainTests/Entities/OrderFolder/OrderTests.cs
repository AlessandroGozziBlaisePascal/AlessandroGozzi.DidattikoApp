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

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.OrderFolder
{
    public class OrderTests
    {
        private CreditCard GetValidCreditCard() => CreditCard.Create("Mario", "Rossi", "12/30", "1234").Value;

        private OrderItem GetValidOrderItem(decimal amount = 29.99m)
        {
            var money = Money.Create(amount).Value;
            return OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Il Signore degli Anelli", money, 1).Value;
        }

        #region OrderItem Tests

        [Fact]
        public void OrderItem_Create_WithValidData_ShouldSucceed()
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

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookTitle");
        }

        [Fact]
        public void OrderItem_Create_WithNullPrice_ShouldFail()
        {
            var result = OrderItem.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Clean Code",
                null!,
                2
            );

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookPrice");
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
        public void Order_Create_WithValidData_ShouldSucceedAndCalculateTotalPrice()
        {
            var customerId = Guid.NewGuid();
            var date = DateTime.UtcNow;
            var card = GetValidCreditCard();
            var items = new List<OrderItem>
        {
            GetValidOrderItem(10.00m),
            GetValidOrderItem(20.50m)
        };

            var result = Order.Create(customerId, card, items);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.CustomerId.Should().Be(customerId);
            result.Value.Status.Should().Be(OrderStatus.Placed);
            result.Value.TotalPrice.Amount.Should().Be(30.50m);
            result.Value._domainEvents.Should().HaveCount(1); 
        }

        [Fact]
        public void Order_Create_WithNullPaymentDetails_ShouldFail()
        {
            var items = new List<OrderItem> { GetValidOrderItem() };

            var result = Order.Create(Guid.NewGuid(), null!, items);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order payment details");
        }

        [Fact]
        public void Order_Create_WithEmptyOrNullItems_ShouldFail()
        {
            var card = GetValidCreditCard();

            var resultEmpty = Order.Create(Guid.NewGuid(), card, new List<OrderItem>());

            var resultNull = Order.Create(Guid.NewGuid(), card, null!);

            resultEmpty.IsFailure.Should().BeTrue();
            resultEmpty.Error.Code.Should().Be("Order items");

            resultNull.IsFailure.Should().BeTrue();
            resultNull.Error.Code.Should().Be("Order items");
        }

        #endregion

        #region Order State Transition Tests

        [Fact]
        public void Order_FullLifecycle_ShouldSucceedInCorrectSequence()
        {
            var card = GetValidCreditCard();
            var items = new List<OrderItem> { GetValidOrderItem() };
            var order = Order.Create(Guid.NewGuid(), card, items).Value;

            var confirmResult = order.Confirm();
            confirmResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Confirmed);

            var prepareResult = order.MarkAsPrepared();
            prepareResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Prepared);

            var shipResult = order.Ship("TRACK12345");
            shipResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Shipped);
            order.TrackingCode.Should().Be("TRACK12345");

            var deliverResult = order.MarkAsDelivered();
            deliverResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Delivered);
        }

        [Fact]
        public void Confirm_WhenStatusIsNotPlaced_ShouldFail()
        {
            var order = Order.Create(Guid.NewGuid(), GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;
            order.Confirm(); 

            var result = order.Confirm();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order confirmation");
        }

        [Fact]
        public void Ship_WithNullTrackingCode_ShouldFail()
        {
            var order = Order.Create(Guid.NewGuid(), GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;
            order.Confirm();
            order.MarkAsPrepared();

            var result = order.Ship(null!);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tracking code");
        }

        [Fact]
        public void CancelOrder_BeforeShipping_ShouldSucceed()
        {
            var order = Order.Create(Guid.NewGuid(), GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;

            var result = order.CancelOrder();

            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Cancelled);
        }

        [Fact]
        public void CancelOrder_WhenAlreadyShipped_ShouldFail()
        {
            var order = Order.Create(Guid.NewGuid(), GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;
            order.Confirm();
            order.MarkAsPrepared();
            order.Ship("TRACK123"); 

            var result = order.CancelOrder();

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order status");
        }

        #endregion

    }
}
