using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Books.ValueObjects;
using AlessandroGozzi_BookECommerce.Domain.AggregateRoots.Carts;
using FluentAssertions;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;

namespace AlessandroGozzi.BookECommerce.DomainTests.AggregateRoots.Carts
{
    public class CartTests
    {
        private static Money ValidPrice => Money.Create(15.00m).Value;
        private static ImageUrl ValidUrl => ImageUrl.Create("cover.jpg").Value;
        private static ImageUrl ValidUrl2 => ImageUrl.Create("cover2.jpg").Value;

        // ==========================================
        // CREATE TESTS
        // ==========================================
        [Fact]
        public void Create_WithValidCustomerId_ShouldReturnSuccess()
        {
            var customerId = Guid.NewGuid();

            var result = Cart.Create(customerId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.CustomerId.Should().Be(customerId);
            result.Value.GetItems.Should().BeEmpty();
        }

        [Fact]
        public void Create_WhenCustomerIdIsEmpty_ShouldReturnFailure()
        {
            var result = Cart.Create(Guid.Empty);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Customer id");
            result.Error.Description.Should().Be("Id is empty");
        }

        // ==========================================
        // ADD ITEM TESTS
        // ==========================================
        [Fact]
        public void AddItem_WhenItemDoesNotExist_ShouldAddNewItemToCart()
        {
            var cart = Cart.Create(Guid.NewGuid()).Value;
            var bookId = Guid.NewGuid();

            var result = cart.AddItem(bookId, Guid.NewGuid(), "Clean Code", ValidPrice, ValidUrl);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().ContainSingle();
            cart.GetItems.First().BookId.Should().Be(bookId);
            cart.GetItems.First().Quantity.Should().Be(1);
        }

        [Fact]
        public void AddItem_WhenItemAlreadyExists_ShouldIncrementQuantity()
        {
            var cart = Cart.Create(Guid.NewGuid()).Value;
            var bookId = Guid.NewGuid();
            var sellerId = Guid.NewGuid();

            cart.AddItem(bookId, sellerId, "Clean Code", ValidPrice, ValidUrl);
            var result = cart.AddItem(bookId, sellerId, "Clean Code", ValidPrice, ValidUrl);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().ContainSingle();
            cart.GetItems.First().Quantity.Should().Be(2);
        }

        // ==========================================
        // REMOVE ITEM TESTS
        // ==========================================
        [Fact]
        public void RemoveItem_WhenItemExists_ShouldRemoveFromCart()
        {
            var cart = Cart.Create(Guid.NewGuid()).Value;
            var bookId = Guid.NewGuid();
            cart.AddItem(bookId, Guid.NewGuid(), "Clean Code", ValidPrice, ValidUrl);

            var result = cart.RemoveItem(bookId);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().BeEmpty();
        }

        [Fact]
        public void RemoveItem_WhenItemDoesNotExist_ShouldReturnNotFoundError()
        {
            var cart = Cart.Create(Guid.NewGuid()).Value;

            var result = cart.RemoveItem(Guid.NewGuid());

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
            result.Error.Description.Should().Be("Product not found in the shopping cart.");
            result.Error.Type.Should().Be(ErrorType.NotFound);
        }

        // ==========================================
        // UPDATE ITEM QUANTITY TESTS
        // ==========================================
        [Fact]
        public void UpdateItemQuantity_WhenItemExists_ShouldUpdateQuantity()
        {
            var cart = Cart.Create(Guid.NewGuid()).Value;
            var bookId = Guid.NewGuid();
            cart.AddItem(bookId, Guid.NewGuid(), "Clean Code", ValidPrice, ValidUrl);

            var result = cart.UpdateItemQuantity(bookId, 3);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.First().Quantity.Should().Be(3);
        }

        [Fact]
        public void UpdateItemQuantity_WhenQuantityIsZero_ShouldRemoveItemFromCart()
        {
            var cart = Cart.Create(Guid.NewGuid()).Value;
            var bookId = Guid.NewGuid();
            cart.AddItem(bookId, Guid.NewGuid(), "Clean Code", ValidPrice, ValidUrl);

            var result = cart.UpdateItemQuantity(bookId, 0);

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().BeEmpty();
        }

        [Fact]
        public void UpdateItemQuantity_WhenItemDoesNotExist_ShouldReturnNotFoundError()
        {
            var cart = Cart.Create(Guid.NewGuid()).Value;

            var result = cart.UpdateItemQuantity(Guid.NewGuid(), 2);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book");
            result.Error.Description.Should().Be("Product not found in the shopping cart.");
            result.Error.Type.Should().Be(ErrorType.NotFound);
        }

        // ==========================================
        // CLEAR CART TESTS
        // ==========================================
        [Fact]
        public void ClearCart_ShouldRemoveAllItemsFromCart()
        {
            var cart = Cart.Create(Guid.NewGuid()).Value;
            cart.AddItem(Guid.NewGuid(), Guid.NewGuid(), "Book 1", ValidPrice, ValidUrl);
            cart.AddItem(Guid.NewGuid(), Guid.NewGuid(), "Book 2", ValidPrice, ValidUrl2);

            var result = cart.ClearCart();

            result.IsSuccess.Should().BeTrue();
            cart.GetItems.Should().BeEmpty();
        }
    }
}
