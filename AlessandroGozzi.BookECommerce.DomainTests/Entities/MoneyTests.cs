using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using AlessandroGozzi_BookECommerce.Domain.Entities;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities
{
    public class MoneyTests
    {
        #region Create Tests

        [Theory]
        [InlineData(0)]
        [InlineData(10.50)]
        [InlineData(100)]
        public void Money_Create_WithValidAmount_ShouldSucceed(decimal validAmount)
        {
            // Act
            var result = Money.Create(validAmount);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Amount.Should().Be(validAmount);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-10)]
        [InlineData(-100.50)]
        public void Money_Create_WithNegativeAmount_ShouldFail(decimal negativeAmount)
        {
            // Act
            var result = Money.Create(negativeAmount);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Money amount");
        }

        #endregion

        #region Operator Tests

        [Fact]
        public void Money_AdditionOperator_ShouldReturnCorrectSum()
        {
            // Arrange
            var money1 = Money.Create(10.50m).Value;
            var money2 = Money.Create(5.25m).Value;

            // Act
            var total = money1 + money2;

            // Assert
            total.Amount.Should().Be(15.75m);
        }

        [Fact]
        public void Money_MultiplyOperator_ShouldReturnCorrectProduct()
        {
            // Arrange
            var money = Money.Create(12.50m).Value;
            var multiplier = 3;

            // Act
            var total = money * multiplier;

            // Assert
            total.Amount.Should().Be(37.50m);
        }

        #endregion

    }
}
