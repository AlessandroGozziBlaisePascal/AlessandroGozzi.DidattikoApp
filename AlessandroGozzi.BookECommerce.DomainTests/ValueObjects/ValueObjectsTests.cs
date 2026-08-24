using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.ValueObjects;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.ValueObjects
{
    public class ValueObjectsTests
    {
        public class CreditCardTests
        {
            private const string ValidName = "Mario";
            private const string ValidSurname = "Rossi";
            private const string ValidExpiry = "12/28";
            private const string ValidLast4Digits = "1234";

            // ==========================================
            // CREATE TESTS
            // ==========================================
            [Fact]
            public void Create_WithValidParameters_ShouldReturnSuccess()
            {
                var customerId = Guid.NewGuid();

                var result = CreditCard.Create(ValidName, ValidSurname, ValidExpiry, ValidLast4Digits, customerId);

                result.IsSuccess.Should().BeTrue();
                result.Value.Should().NotBeNull();
                result.Value.CustomerId.Should().Be(customerId);
                result.Value.Last4Digits.Should().Be(ValidLast4Digits);
                result.Value.DisplayName.Should().Be("XXXX-XXXX-XXXX-1234");
                result.Value.CardOwner.Name.Value.Should().Be(ValidName);
                result.Value.CardOwner.Surname.Value.Should().Be(ValidSurname);
            }

            [Fact]
            public void Create_WhenOwnerNameIsInvalid_ShouldReturnFailure()
            {
                var result = CreditCard.Create("", ValidSurname, ValidExpiry, ValidLast4Digits, Guid.NewGuid());

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Owner name");
                result.Error.Description.Should().Be("Incorrect owner name");
                result.Error.Type.Should().Be(ErrorType.Validation);
            }

            [Fact]
            public void Create_WhenOwnerSurnameIsInvalid_ShouldReturnFailure()
            {
                var result = CreditCard.Create(ValidName, "", ValidExpiry, ValidLast4Digits, Guid.NewGuid());

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Owner surname");
                result.Error.Description.Should().Be("Incorrect owner surname");
                result.Error.Type.Should().Be(ErrorType.Validation);
            }

            [Fact]
            public void Create_WhenExpiryDateIsInvalid_ShouldReturnFailure()
            {
                var result = CreditCard.Create(ValidName, ValidSurname, "invalid-date", ValidLast4Digits, Guid.NewGuid());

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Expiry date");
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            [InlineData("123")] // Meno di 4 cifre
            [InlineData("12345")] // Più di 4 cifre
            [InlineData("12a4")] // Contiene lettere
            public void Create_WhenLast4DigitsAreInvalid_ShouldReturnFailure(string? invalidLast4Digits)
            {
                var result = CreditCard.Create(ValidName, ValidSurname, ValidExpiry, invalidLast4Digits!, Guid.NewGuid());

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Last 4 digits card");
                result.Error.Description.Should().Be("Card must have 4 last digits");
                result.Error.Type.Should().Be(ErrorType.Validation);
            }

            [Fact]
            public void Create_WhenCustomerIdIsEmpty_ShouldReturnFailure()
            {
                var result = CreditCard.Create(ValidName, ValidSurname, ValidExpiry, ValidLast4Digits, Guid.Empty);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Customer id");
                result.Error.Description.Should().Be("Customer id cannot be null");
                result.Error.Type.Should().Be(ErrorType.Validation);
            }

            // ==========================================
            // IS EXPIRED TESTS
            // ==========================================
            [Fact]
            public void IsExpired_WhenCardIsExpired_ShouldReturnTrue()
            {
                var creditCard = CreditCard.Create(ValidName, ValidSurname, "01/22", ValidLast4Digits, Guid.NewGuid()).Value;
                var referenceDate = new DateTime(2024, 01, 01);

                var isExpired = creditCard.IsExpired(referenceDate);

                isExpired.Should().BeTrue();
            }

            [Fact]
            public void IsExpired_WhenCardIsNotExpired_ShouldReturnFalse()
            {
                var creditCard = CreditCard.Create(ValidName, ValidSurname, "12/28", ValidLast4Digits, Guid.NewGuid()).Value;
                var referenceDate = new DateTime(2024, 01, 01);

                var isExpired = creditCard.IsExpired(referenceDate);

                isExpired.Should().BeFalse();
            }
        }
        public class CreditCardValueObjectTests
        {
            // ==========================================
            // CREATE TESTS
            // ==========================================
            [Theory]
            [InlineData("12/28", 12, 2028)]
            [InlineData("01/25", 1, 2025)]
            [InlineData("09/30", 9, 2030)]
            public void Create_WithValidFormat_ShouldReturnSuccess(string rawDate, int expectedMonth, int expectedYear)
            {
                var result = ExpiryDate.Create(rawDate);

                result.IsSuccess.Should().BeTrue();
                result.Value.Should().NotBeNull();
                result.Value.Month.Should().Be(expectedMonth);
                result.Value.Year.Should().Be(expectedYear);
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            [InlineData("13/28")] // Mese non valido
            [InlineData("00/28")] // Mese non valido
            [InlineData("12/2028")] // Anno a 4 cifre invece di 2
            [InlineData("12-28")] // Separatore errato
            [InlineData("abc")]
            public void Create_WithInvalidFormat_ShouldReturnFailure(string? invalidDate)
            {
                var result = ExpiryDate.Create(invalidDate!);

                result.IsFailure.Should().BeTrue();
                result.Error.Code.Should().Be("Expiry date");
                result.Error.Description.Should().Be("No valid format");
                result.Error.Type.Should().Be(ErrorType.Validation);
            }

            // ==========================================
            // IS EXPIRED TESTS
            // ==========================================
            [Fact]
            public void IsExpired_WhenDateIsInThePast_ShouldReturnTrue()
            {
                var expiryDate = ExpiryDate.Create("05/24").Value;
                var referenceDate = new DateTime(2024, 06, 01);

                var isExpired = expiryDate.IsExpired(referenceDate);

                isExpired.Should().BeTrue();
            }

            [Fact]
            public void IsExpired_WhenDateIsCurrentMonth_ShouldReturnFalse()
            {
                var expiryDate = ExpiryDate.Create("05/24").Value;
                var referenceDate = new DateTime(2024, 05, 15);

                var isExpired = expiryDate.IsExpired(referenceDate);

                isExpired.Should().BeFalse();
            }

            [Fact]
            public void IsExpired_WhenDateIsInTheFuture_ShouldReturnFalse()
            {
                var expiryDate = ExpiryDate.Create("12/28").Value;
                var referenceDate = new DateTime(2024, 05, 15);

                var isExpired = expiryDate.IsExpired(referenceDate);

                isExpired.Should().BeFalse();
            }

            // ==========================================
            // TO STRING TESTS
            // ==========================================
            [Fact]
            public void ToString_ShouldReturnFormattedString()
            {
                var expiryDate = ExpiryDate.Create("08/26").Value;

                var result = expiryDate.ToString();

                result.Should().Be("8/2026");
            }
        }
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
}
