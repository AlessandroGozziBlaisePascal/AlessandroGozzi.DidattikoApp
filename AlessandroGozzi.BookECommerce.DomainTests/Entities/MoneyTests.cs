using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities
{
    public class MoneyTests
    {
        // ==========================================
        // CREATE TESTS
        // ==========================================
        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(10.50)]
        [InlineData(1000)]
        public void Create_WithZeroOrPositiveAmount_ShouldReturnSuccess(decimal validAmount)
        {
            var result = Money.Create(validAmount);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Amount.Should().Be(validAmount);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-10.50)]
        [InlineData(-100)]
        public void Create_WithNegativeAmount_ShouldReturnFailure(decimal negativeAmount)
        {
            var result = Money.Create(negativeAmount);

            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Money amount");
            result.Error.Description.Should().Be("Amount cannot be negative");
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        // ==========================================
        // OPERATOR TESTS
        // ==========================================
        [Fact]
        public void AdditionOperator_ShouldAddAmountsCorrectly()
        {
            var moneyA = Money.Create(10.50m).Value;
            var moneyB = Money.Create(5.25m).Value;

            var result = moneyA + moneyB;

            result.Should().NotBeNull();
            result.Amount.Should().Be(15.75m);
        }

        [Fact]
        public void SubtractionOperator_ShouldSubtractAmountsCorrectly()
        {
            var moneyA = Money.Create(20.00m).Value;
            var moneyB = Money.Create(7.50m).Value;

            var result = moneyA - moneyB;

            result.Should().NotBeNull();
            result.Amount.Should().Be(12.50m);
        }

        [Fact]
        public void MultiplicationOperator_ShouldMultiplyAmountByIntegerCorrectly()
        {
            var money = Money.Create(12.50m).Value;
            const int multiplier = 3;

            var result = money * multiplier;

            result.Should().NotBeNull();
            result.Amount.Should().Be(37.50m);
        }

        // ==========================================
        // RECORD EQUALITY TESTS
        // ==========================================
        [Fact]
        public void Equals_WhenAmountsAreSame_ShouldBeEqual()
        {
            var moneyA = Money.Create(15.00m).Value;
            var moneyB = Money.Create(15.00m).Value;

            moneyA.Should().Be(moneyB);
            (moneyA == moneyB).Should().BeTrue();
        }
    }
}
