using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder.Event;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CartFolder
{
    public class CartTests
    {
        private readonly Guid _validCustomerId = Guid.NewGuid();
        private readonly Guid _validBookId = Guid.NewGuid();
        private readonly string _validTitle = "Clean Code";
        private readonly Money _validPrice = Money.Create(29.99m).Value;
        private readonly string _validPhoto = "cover.jpg";

        private Cart CreateValidCart()
        {
            return Cart.Create(_validCustomerId).Value;
        }

        #region Create Tests

        [Fact]
        public void Create_ShouldFail_WhenCustomerIdIsEmpty()
        {
            var result = Cart.Create(Guid.Empty);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Customer id");
        }

        [Fact]
        public void Create_ShouldSucceed_WhenCustomerIdIsValid()
        {
            var result = Cart.Create(_validCustomerId);

            result.IsSuccess.Should().BeTrue();
            result.Value.CustomerId.Should().Be(_validCustomerId);
            result.Value.GetItems.Should().BeEmpty();
        }

        #endregion

        #region AddItem Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddItem_ShouldFail_WhenQuantityIsZeroOrNegative(int invalidQuantity)
        {
            var cart = CreateValidCart();

            var result = cart.AddItem(_validBookId, _validTitle, _validPrice, _validPhoto, invalidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
        }

        [Fact]
        public void AddItem_ShouldFail_WhenCartItemCreationFails()
        {
            var cart = CreateValidCart();

            var result = cart.AddItem(_validBookId, "", _validPrice, _validPhoto, 1);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void AddItem_ShouldAddNewItem_WhenItemNotInCart()
        {
            var cart = CreateValidCart();

            var result = cart.AddItem(_validBookId, _validTitle, _validPrice, _validPhoto, 2);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().HaveCount(1);
        }

        [Fact]
        public void AddItem_ShouldUpdateQuantity_WhenItemAlreadyInCart()
        {
            var cart = CreateValidCart();
            cart.AddItem(_validBookId, _validTitle, _validPrice, _validPhoto, 2);

            var result = cart.AddItem(_validBookId, _validTitle, _validPrice, _validPhoto, 3);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().HaveCount(1);
        }

        #endregion

        #region RemoveItem Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RemoveItem_ShouldFail_WhenQuantityIsZeroOrNegative(int invalidQuantity)
        {
            var cart = CreateValidCart();

            var result = cart.RemoveItem(_validBookId, invalidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
        }

        [Fact]
        public void RemoveItem_ShouldFail_WhenItemNotFound()
        {
            var cart = CreateValidCart();

            var result = cart.RemoveItem(Guid.NewGuid(), 1);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
        }

        [Fact]
        public void RemoveItem_ShouldFail_WhenQuantityToRemoveIsGreaterThanCartQuantity()
        {
            var cart = CreateValidCart();
            cart.AddItem(_validBookId, _validTitle, _validPrice, _validPhoto, 2);

            var result = cart.RemoveItem(_validBookId, 5);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
        }

        [Fact]
        public void RemoveItem_ShouldReduceQuantity_WhenQuantityToRemoveIsLess()
        {
            var cart = CreateValidCart();
            cart.AddItem(_validBookId, _validTitle, _validPrice, _validPhoto, 5);

            var result = cart.RemoveItem(_validBookId, 2);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().HaveCount(1);
        }

        [Fact]
        public void RemoveItem_ShouldRemoveItemEntirely_WhenQuantityToRemoveEqualsCartQuantity()
        {
            var cart = CreateValidCart();
            cart.AddItem(_validBookId, _validTitle, _validPrice, _validPhoto, 2);

            var result = cart.RemoveItem(_validBookId, 2);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().BeEmpty();
        }

        #endregion

        #region ClearCart Tests

        [Fact]
        public void ClearCart_ShouldRemoveAllItems()
        {
            var cart = CreateValidCart();
            cart.AddItem(_validBookId, _validTitle, _validPrice, _validPhoto, 2);
            cart.AddItem(Guid.NewGuid(), "Another Book", _validPrice, _validPhoto, 1);

            var result = cart.ClearCart();

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().BeEmpty();
        }

        #endregion

    }
}
