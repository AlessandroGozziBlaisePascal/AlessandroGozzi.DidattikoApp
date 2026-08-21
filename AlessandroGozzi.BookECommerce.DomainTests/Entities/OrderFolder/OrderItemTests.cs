using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.OrderFolder
{
    public class OrderItemTests
    {
        private readonly Guid _validBookId = Guid.NewGuid();
        private readonly Guid _validSellerId = Guid.NewGuid();
        private const string ValidBookTitle = " Il Signore degli Anelli ";
        private readonly Money _validPrice = Money.Create(15.50m).Value;
        private const int ValidQuantity = 2;

        // ==========================================
        // CREATE TESTS (SUCCESS)
        // ==========================================
        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccessAndCalculateTotalPrice()
        {
            var result = OrderItem.Create(_validBookId, _validSellerId, ValidBookTitle, _validPrice, ValidQuantity);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.BookId.Should().Be(_validBookId);
            result.Value.SellerId.Should().Be(_validSellerId);
            result.Value.BookTitle.Should().Be(ValidBookTitle);
            result.Value.Price.Should().Be(_validPrice);
            result.Value.Quantity.Should().Be(ValidQuantity);
            result.Value.TotalPrice.Amount.Should().Be(31.00m); // 15.50 * 2
        }

        // ==========================================
        // CREATE TESTS (VALIDATIONS / FAILURES)
        // ==========================================
        [Fact]
        public void Create_WhenBookIdIsEmpty_ShouldReturnFailure()
        {
            var result = OrderItem.Create(Guid.Empty, _validSellerId, ValidBookTitle, _validPrice, ValidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookId");
            result.Error.Description.Should().Be("Book ID cannot be empty");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenSellerIdIsEmpty_ShouldReturnFailure()
        {
            var result = OrderItem.Create(_validBookId, Guid.Empty, ValidBookTitle, _validPrice, ValidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("SellerId");
            result.Error.Description.Should().Be("Seller ID cannot be empty");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WhenBookTitleIsNullOrEmptyOrWhitespace_ShouldReturnFailure(string? invalidTitle)
        {
            var result = OrderItem.Create(_validBookId, _validSellerId, invalidTitle!, _validPrice, ValidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookTitle");
            result.Error.Description.Should().Be("Title cannot be null or whitespace");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Create_WhenPriceIsNull_ShouldReturnFailure()
        {
            var result = OrderItem.Create(_validBookId, _validSellerId, ValidBookTitle, null!, ValidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("BookPrice");
            result.Error.Description.Should().Be("Price cannot be null");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Create_WhenQuantityIsLessThanOne_ShouldReturnFailure(int invalidQuantity)
        {
            var result = OrderItem.Create(_validBookId, _validSellerId, ValidBookTitle, _validPrice, invalidQuantity);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Quantity");
            result.Error.Description.Should().Be("Quantity must be at least 1");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
    }
}
