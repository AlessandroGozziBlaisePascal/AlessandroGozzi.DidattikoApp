using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using AlessandroGozzi_BookECommerce.Domain.Entities.CartFolder;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CartFolder
{
    public class CartItemTests
    {
        private static Money CreateValidMoney(decimal amount = 10.0m) => Money.Create(amount).Value;

        #region Factory Method (Create) Tests - Success

        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccessResultWithCartItem()
        {
            var bookId = Guid.NewGuid();
            var title = "Il Nome della Rosa";
            var price = CreateValidMoney(15.99m);
            var mainPhoto = "https://example.com/photo.jpg";
            var quantity = 2;

            var result = CartItem.Create(bookId, title, price, mainPhoto, quantity);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.BookId.Should().Be(bookId);
            result.Value.BookTitle.Should().Be(title);
            result.Value.Price.Should().Be(price);
            result.Value.MainPhoto.Should().Be(mainPhoto);
            result.Value.Quantity.Should().Be(quantity);
        }

        #endregion

        #region Factory Method (Create) Tests - Validations

        [Fact]
        public void Create_WithEmptyBookId_ShouldReturnFailureError()
        {
            var result = CartItem.Create(
                Guid.Empty,
                "Valid Title",
                CreateValidMoney(),
                "photo.jpg",
                1);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book id");
            result.Error.Description.Should().Be("Book id is empty");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidTitle_ShouldReturnFailureError(string? invalidTitle)
        {
            var result = CartItem.Create(
                Guid.NewGuid(),
                invalidTitle!,
                CreateValidMoney(),
                "photo.jpg",
                1);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book title");
            result.Error.Description.Should().Be("Book title is empty");
        }

        [Fact]
        public void Create_WithNullPrice_ShouldReturnFailureError()
        {
            var result = CartItem.Create(
                Guid.NewGuid(),
                "Valid Title",
                null!,
                "photo.jpg",
                1);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Price");
            result.Error.Description.Should().Be("Price is invalid");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidMainPhoto_ShouldReturnFailureError(string? invalidMainPhoto)
        {
            var result = CartItem.Create(
                Guid.NewGuid(),
                "Valid Title",
                CreateValidMoney(),
                invalidMainPhoto!,
                1);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Main photo");
            result.Error.Description.Should().Be("Main photo is empty");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Create_WithQuantityZeroOrNegative_ShouldReturnFailureError(int invalidQuantity)
        {
            var result = CartItem.Create(
                Guid.NewGuid(),
                "Valid Title",
                CreateValidMoney(),
                "photo.jpg",
                invalidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Quantity");
            result.Error.Description.Should().Be("Quantity must be greater than zero.");
        }

        #endregion

        #region UpdateQuantity Tests

        [Fact]
        public void UpdateQuantity_ShouldUpdateQuantityProperty()
        {
            var cartItem = CartItem.Create(
                Guid.NewGuid(),
                "Title",
                CreateValidMoney(),
                "photo.jpg",
                1).Value;

            var newQuantity = 5;

            cartItem.UpdateQuantity(newQuantity);

            cartItem.Quantity.Should().Be(newQuantity);
        }

        #endregion
    }
}
