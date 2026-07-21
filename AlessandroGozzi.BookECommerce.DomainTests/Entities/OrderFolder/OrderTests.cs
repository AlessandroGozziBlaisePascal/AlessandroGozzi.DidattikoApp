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
        // Helper per creare un CreditCard fittizio (o mockato) per i test
        private CreditCard GetValidCreditCard() => CreditCard.Create("Mario", "Rossi", "12/30", "1234", "token_1029303").Value;

        // Helper per creare un OrderItem valido
        private OrderItem GetValidOrderItem(decimal amount = 29.99m)
        {
            var money = Money.Create(amount).Value;
            return OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Il Signore degli Anelli", money).Value;
        }

        #region OrderItem Tests

        [Fact]
        public void OrderItem_Create_WithValidData_ShouldSucceed()
        {
            // Act
            var result = OrderItem.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Design Patterns",
                Money.Create(49.99m).Value
            );

            // Assert
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
            // Act
            var result = OrderItem.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                invalidTitle!,
                Money.Create(10m).Value
            );

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookTitle");
        }

        [Fact]
        public void OrderItem_Create_WithNullPrice_ShouldFail()
        {
            // Act
            var result = OrderItem.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Clean Code",
                null!
            );

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookPrice");
        }

        #endregion

        #region Order Creation & Total Price Tests

        [Fact]
        public void Order_Create_WithValidData_ShouldSucceedAndCalculateTotalPrice()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var date = DateTime.UtcNow;
            var card = GetValidCreditCard();
            var items = new List<OrderItem>
        {
            GetValidOrderItem(10.00m),
            GetValidOrderItem(20.50m)
        };

            // Act
            var result = Order.Create(customerId, date, card, items);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.CustomerId.Should().Be(customerId);
            result.Value.Status.Should().Be(OrderStatus.Placed);
            result.Value.TotalPrice.Amount.Should().Be(30.50m);
            result.Value._domainEvents.Should().HaveCount(1); // OrderPlacedEvent
        }

        [Fact]
        public void Order_Create_WithNullPaymentDetails_ShouldFail()
        {
            // Arrange
            var items = new List<OrderItem> { GetValidOrderItem() };

            // Act
            var result = Order.Create(Guid.NewGuid(), DateTime.UtcNow, null!, items);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order payment details");
        }

        [Fact]
        public void Order_Create_WithEmptyOrNullItems_ShouldFail()
        {
            // Arrange
            var card = GetValidCreditCard();

            // Act 1: Lista vuota
            var resultEmpty = Order.Create(Guid.NewGuid(), DateTime.UtcNow, card, new List<OrderItem>());

            // Act 2: Lista null
            var resultNull = Order.Create(Guid.NewGuid(), DateTime.UtcNow, card, null!);

            // Assert
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
            // Arrange: Creazione dell'ordine (Stato: Placed)
            var card = GetValidCreditCard();
            var items = new List<OrderItem> { GetValidOrderItem() };
            var order = Order.Create(Guid.NewGuid(), DateTime.UtcNow, card, items).Value;

            // Act & Assert 1: Confirm (Placed -> Confirmed)
            var confirmResult = order.Confirm();
            confirmResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Confirmed);

            // Act & Assert 2: MarkAsPrepared (Confirmed -> Prepared)
            var prepareResult = order.MarkAsPrepared();
            prepareResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Prepared);

            // Act & Assert 3: Ship (Prepared -> Shipped)
            var shipResult = order.Ship("TRACK12345");
            shipResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Shipped);
            order.TrackingCode.Should().Be("TRACK12345");

            // Act & Assert 4: MarkAsDelivered (Shipped -> Delivered)
            var deliverResult = order.MarkAsDelivered();
            deliverResult.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Delivered);
        }

        [Fact]
        public void Confirm_WhenStatusIsNotPlaced_ShouldFail()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid(), DateTime.UtcNow, GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;
            order.Confirm(); // Stato ora è Confirmed

            // Act: proviamo a rifare Confirm
            var result = order.Confirm();

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order confirmation");
        }

        [Fact]
        public void Ship_WithNullTrackingCode_ShouldFail()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid(), DateTime.UtcNow, GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;
            order.Confirm();
            order.MarkAsPrepared();

            // Act
            var result = order.Ship(null!);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Tracking code");
        }

        [Fact]
        public void CancelOrder_BeforeShipping_ShouldSucceed()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid(), DateTime.UtcNow, GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;

            // Act
            var result = order.CancelOrder();

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Cancelled);
        }

        [Fact]
        public void CancelOrder_WhenAlreadyShipped_ShouldFail()
        {
            // Arrange: Portiamo l'ordine fino allo stato Shipped
            var order = Order.Create(Guid.NewGuid(), DateTime.UtcNow, GetValidCreditCard(), new List<OrderItem> { GetValidOrderItem() }).Value;
            order.Confirm();
            order.MarkAsPrepared();
            order.Ship("TRACK123"); // Ora TrackingCode != null

            // Act
            var result = order.CancelOrder();

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Order status");
        }

        #endregion

    }
}
