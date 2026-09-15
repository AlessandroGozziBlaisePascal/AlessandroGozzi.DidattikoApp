using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Books.ValueObjects;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Carts;
using AlessandroGozzi.BookECommerce.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.AggregateRoots.Carts
{
    public class CartItemTests
    {
        private static Money ValidPrice => Money.Create(15.00m).Value;
        private static ImageUrl ValidUrl => ImageUrl.Create("cover.jpg").Value;

        // ==========================================
        // CREATE TESTS
        // ==========================================
        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccess()
        {
            var bookId = Guid.NewGuid();
            var sellerId = Guid.NewGuid();

            var result = CartItem.Create(bookId, sellerId, "Clean Code", ValidPrice, ValidUrl);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.BookId.Should().Be(bookId);
            result.Value.SellerId.Should().Be(sellerId);
            result.Value.BookTitle.Should().Be("Clean Code");
            result.Value.Price.Should().Be(ValidPrice);
            result.Value.MainPhoto.Value.Should().Be("cover.jpg");
            result.Value.Quantity.Should().Be(1);
        }

        [Fact]
        public void Create_WhenBookIdIsEmpty_ShouldReturnFailure()
        {
            var result = CartItem.Create(Guid.Empty, Guid.NewGuid(), "Clean Code", ValidPrice, ValidUrl);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book id");
            result.Error.Description.Should().Be("Book id is empty");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenSellerIdIsEmpty_ShouldReturnFailure()
        {
            var result = CartItem.Create(Guid.NewGuid(), Guid.Empty, "Clean Code", ValidPrice, ValidUrl);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Seller id");
            result.Error.Description.Should().Be("Seller id is empty");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WhenTitleIsNullOrEmpty_ShouldReturnFailure(string? invalidTitle)
        {
            var result = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), invalidTitle!, ValidPrice,ValidUrl);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Book title");
            result.Error.Description.Should().Be("Book title is empty");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenPriceIsNull_ShouldReturnFailure()
        {
            var result = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Clean Code", null!, ValidUrl);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Price");
            result.Error.Description.Should().Be("Price is invalid");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenPriceAmountIsZeroOrNegative_ShouldReturnFailure()
        {
            var invalidPrice = Money.Create(0.00m).Value;

            var result = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Clean Code", invalidPrice,ValidUrl);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Price");
            result.Error.Description.Should().Be("Price is invalid");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        // ==========================================
        // UPDATE QUANTITY TESTS
        // ==========================================
        [Fact]
        public void UpdateQuantity_WithValidQuantity_ShouldReturnSuccessAndSetQuantity()
        {
            var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Clean Code", ValidPrice, ValidUrl).Value;

            var result = cartItem.UpdateQuantity(5);

            result.IsSuccess.Should().BeTrue();
            cartItem.Quantity.Should().Be(5);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)]
        public void UpdateQuantity_WhenQuantityIsZeroOrLess_ShouldReturnFailure(int invalidQuantity)
        {
            var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), "Clean Code", ValidPrice, ValidUrl).Value;

            var result = cartItem.UpdateQuantity(invalidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Quantity");
            result.Error.Description.Should().Be("Quantity must be greater than 1");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
    }
}
