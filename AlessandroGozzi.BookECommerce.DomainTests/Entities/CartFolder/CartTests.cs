using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Event;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CartFolder
{
    public class CartTests
    {
        private readonly Guid _validCustomerId = Guid.NewGuid();
        private readonly Guid _validBookId = Guid.NewGuid();

        #region Create Tests

        [Fact]
        public void Create_WithValidCustomerId_ShouldCreateEmptyCart()
        {
            var result = Cart.Create(_validCustomerId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.CustomerId.Should().Be(_validCustomerId);
            result.Value.GetItems.Should().BeEmpty();
        }

        [Fact]
        public void Create_WithEmptyCustomerId_ShouldReturnFailure()
        {
            var result = Cart.Create(Guid.Empty);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Customer id");
        }

        #endregion

        #region AddBook Tests

        [Fact]
        public void AddBook_NewItem_ShouldAddItemAndRaiseDomainEvent()
        {
            var cart = Cart.Create(_validCustomerId).Value;
            int quantity = 2;

            var result = cart.AddBook(_validBookId, quantity);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().HaveCount(1);
            cart.GetItems.Should().ContainSingle(i => i.BookId == _validBookId && i.Quantity == quantity);

            cart._domainEvents.Should().ContainSingle(e => e is BookAddedToCartEvent);
        }

        [Fact]
        public void AddBook_ExistingItem_ShouldUpdateQuantity()
        {
            var cart = Cart.Create(_validCustomerId).Value;
            cart.AddBook(_validBookId, 2);

            var result = cart.AddBook(_validBookId, 3); // Aggiunge altre 3 quantità

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().HaveCount(1);
            cart.GetItems.Should().ContainSingle(i => i.BookId == _validBookId && i.Quantity == 5);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)]
        public void AddBook_WithInvalidQuantity_ShouldReturnFailure(int invalidQuantity)
        {
            var cart = Cart.Create(_validCustomerId).Value;

            var result = cart.AddBook(_validBookId, invalidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
            cart.GetItems.Should().BeEmpty();
        }

        #endregion

        #region RemoveBook Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void RemoveBook_WithInvalidQuantity_ShouldReturnFailure(int invalidQuantity)
        {
            var cart = Cart.Create(_validCustomerId).Value;

            var result = cart.RemoveBook(_validBookId, invalidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
        }

        [Fact]
        public void RemoveBook_ItemNotFound_ShouldReturnFailure()
        {
            // Arrange
            var cart = Cart.Create(_validCustomerId).Value;

            // Act
            var result = cart.RemoveBook(_validBookId, 1);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
        }

        [Fact]
        public void RemoveBook_QuantityGreaterThanAvailable_ShouldReturnFailure()
        {
            // Arrange
            var cart = Cart.Create(_validCustomerId).Value;
            cart.AddBook(_validBookId, 2);

            // Act
            var result = cart.RemoveBook(_validBookId, 5); // Tenta di rimuoverne 5 avendone solo 2

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
            cart.GetItems.Should().ContainSingle(i => i.Quantity == 2);
        }

        [Fact]
        public void RemoveBook_PartialQuantity_ShouldDecreaseQuantityAndRaiseEvent()
        {
            // Arrange
            var cart = Cart.Create(_validCustomerId).Value;
            cart.AddBook(_validBookId, 5);

            // Act
            var result = cart.RemoveBook(_validBookId, 2);

            // Assert
            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().ContainSingle(i => i.BookId == _validBookId && i.Quantity == 3);

            // Verifica Evento
            cart._domainEvents.Should().Contain(e => e is BookRemovedFromCartEvent);
        }

        [Fact]
        public void RemoveBook_ExactQuantity_ShouldRemoveItemFromCart()
        {
            // Arrange
            var cart = Cart.Create(_validCustomerId).Value;
            cart.AddBook(_validBookId, 3);

            // Act
            var result = cart.RemoveBook(_validBookId, 3); // Rimuove tutte le quantità

            // Assert
            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().BeEmpty();
        }

        #endregion

        #region ClearCart Tests

        [Fact]
        public void ClearCart_ShouldRemoveAllItems()
        {
            // Arrange
            var cart = Cart.Create(_validCustomerId).Value;
            cart.AddBook(Guid.NewGuid(), 2);
            cart.AddBook(Guid.NewGuid(), 1);

            // Act
            var result = cart.ClearCart();

            // Assert
            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().BeEmpty();
        }

        #endregion

    }
}
