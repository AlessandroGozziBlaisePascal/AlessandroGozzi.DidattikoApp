using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder.Value_Object;
using FluentAssertions;

namespace AlessandroGozzi.BookECommerce.DomainTests.Entities.CreditCardFolder
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
}
